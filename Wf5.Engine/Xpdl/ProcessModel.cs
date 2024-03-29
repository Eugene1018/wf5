﻿using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Diagnostics;
using Wf5.Engine.Utility;
using Wf5.Engine.Common;
using Wf5.Engine.Service;
using Wf5.Engine.Business.Entity;
using Wf5.Engine.Business.Manager;
using Wf5.Engine.Xpdl.Node;

namespace Wf5.Engine.Xpdl
{
    /// <summary>
    /// 流程定义模型类
    /// </summary>
    public class ProcessModel
    {
        #region 属性和构造函数
        internal Guid ProcessGUID { get; private set; }
        internal ProcessEntity ProcessEntity { get; private set; }

        internal XmlDocument XmlProcessDefinition
        {
            get
            {
                if (CachedHelper.GetXpdlCache(ProcessGUID) == null)
                {
                    var xmlDoc = GetXmlProcessDefinitionFromDisk();

                    CachedHelper.SetXpdlCache(ProcessGUID, xmlDoc);
                }
                return CachedHelper.GetXpdlCache(ProcessGUID);
            }
        }

        internal XmlNode _xmlParticipants;
        internal XmlNode XmlParticipants
        {
            get
            {
                if (_xmlParticipants == null)
                {
                    _xmlParticipants = GetXmlParticipants();
                }
                return _xmlParticipants;
            }
        }

        internal XmlNode _xmlDataItems;
        internal XmlNode XmlDataItems
        {
            get
            {
                if (_xmlDataItems == null)
                {
                    _xmlDataItems = GetXmlDataItems();
                }
                return _xmlDataItems;
            }
        }

        internal IXPDLReader XPDLReader
        {
            get;
            set;
        }

        internal ProcessModel(Guid processGUID)
        {
            ProcessGUID = processGUID;
            ProcessEntity = (new ProcessManager()).GetById(processGUID);
            XPDLReader = new XPDLReader();
        }
        #endregion

        #region 获取流程信息
        /// <summary>
        /// 读取流程的配置文件
        /// </summary>
        /// <param name="processGUID"></param>
        /// <returns></returns>
        private XmlDocument GetXmlProcessDefinitionFromDisk()
        {
            XmlDocument xmlDoc = null;
            if (XPDLReader.IsReadable() == false)
            {
                //本地路径存储的文件
                string filePath = ProcessEntity.XmlFilePath;
                string fileName = ProcessEntity.XmlFileName;
                string serverPath = ConfigHelper.GetAppSettingString("WorkflowFileServer");
                string physicalFileName = serverPath + "\\" + filePath + "\\" + fileName;

                xmlDoc = new XmlDocument();
                xmlDoc.Load(physicalFileName);
            }
            else
            {
                //加载其它方式定义的流程XML文件
                xmlDoc = XPDLReader.Read(ProcessEntity);
            }
            return xmlDoc;
        }

        /// <summary>
        /// 获取流程参与者信息
        /// </summary>
        /// <returns></returns>
        private XmlNode GetXmlParticipants()
        {
            return XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition, XPDLDefinition.StrXmlParticipantsPath);
        }

        /// <summary>
        /// 获取流程的数据集合信息
        /// </summary>
        /// <returns></returns>
        private XmlNode GetXmlDataItems()
        {
            return XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition, XPDLDefinition.StrXmlDataItems);
        }
        #endregion

        #region 活动节点基本方法和流转规则处理

        #region 活动节点基本方法
        /// <summary>
        /// 获取开始节点信息
        /// </summary>
        /// <returns></returns>
        internal ActivityEntity GetStartActivity()
        {
            string nodeType = "StartNode";

            XmlNode startTypeNode = GetXmlActivityTypeNodeFromXmlFile(nodeType);
            return ConvertXmlActivityNodeToActivityEntity(startTypeNode.ParentNode);
        }

        /// <summary>
        /// 获取结束节点
        /// </summary>
        /// <returns></returns>
        internal ActivityEntity GetEndActivity()
        {
            string nodeType = "EndNode";
            XmlNode endTypeNode = GetXmlActivityTypeNodeFromXmlFile(nodeType);
            return ConvertXmlActivityNodeToActivityEntity(endTypeNode.ParentNode);
        }

        /// <summary>
        /// 获取流程的第一个可办理节点
        /// </summary>
        /// <returns></returns>
        internal ActivityEntity GetFirstActivity()
        {
            ActivityEntity startActivity = GetStartActivity();
            ActivityEntity firstActivity = GetNextActivity(startActivity.ActivityGUID);
            return firstActivity;
        }

        /// <summary>
        /// 获取当前节点的下一个节点信息
        /// </summary>
        /// <param name="currentActivitystring"></param>
        /// <returns></returns>
        internal ActivityEntity GetNextActivity(Guid activityGUID)
        {
            XmlNode transitionNode = GetForwardXmlTransitionNode(activityGUID);

            return GetActivityFromTransitionTo(transitionNode);
        }

        /// <summary>
        /// 获取流程起始的活动节点列表(开始节点之后，可能有多个节点)
        /// </summary>
        /// <param name="conditionKeyValuePair">条件表达式的参数名称-参数值的集合</param>
        /// <returns></returns>
        internal NextActivityMatchedResult GetFirstActivityList(IDictionary<string, string> conditionKeyValuePair)
        {
            try
            {
                ActivityEntity startActivity = GetStartActivity();
                return GetNextActivityList(startActivity.ActivityGUID, conditionKeyValuePair);
            }
            catch (System.Exception e)
            {
                throw new WfXpdlException(string.Format("解析流程定义文件发生异常，异常描述：{0}", e.Message), e);
            }
        }
        #endregion

        #region 流程流转解析，处理流程下一步流转条件等规则
        /// <summary>
        /// 获取下一步活动节点树，供流转界面使用
        /// </summary>
        /// <param name="task"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        internal IList<NodeView> GetNextActivityTree(int processInstanceID,
            Guid currentActivityGUID, 
            IDictionary<string, string> condition,
            IUserRoleService roleService)
        {
            var activity = GetActivity(currentActivityGUID);
            var nextSteps = GetNextActivityList(activity.ActivityGUID, condition);
            var treeNodeList = new List<NodeView>();

            foreach (var child in nextSteps.Root)
            {
                if (child.HasChildren)
                {
                    Tranverse(child, treeNodeList);
                }
                else
                {
                    PerformerList performerList = null;
                    if (roleService != null)
                    {
                        //读取外部组织关系数据
                        //IUserRoleService userService = container.Resolve<IUserDeptService>();
                        performerList = roleService.GetPerformerList(processInstanceID, 
                            child.Activity.Roles.ToList());
                    }

                    treeNodeList.Add(new NodeView
                    {
                        ID = child.Activity.ActivityGUID,
                        Text = child.Activity.ActivityName,
                        Roles = child.Activity.Roles,
                        PerformerList = performerList
                    });
                }
            }

            return treeNodeList;
        }

        /// <summary>
        /// 迭代遍历
        /// </summary>
        /// <param name="root"></param>
        /// <param name="treeNodeList"></param>
        private void Tranverse(NextActivityComponent root, IList<NodeView> treeNodeList)
        {
            foreach (var child in root)
            {
                if (child.HasChildren)
                {
                    Tranverse(child, treeNodeList);
                }
                else
                {
                    treeNodeList.Add(new NodeView
                    {
                        ID = child.Activity.ActivityGUID,
                        Text = child.Activity.ActivityName,
                        Roles = child.Activity.Roles
                    });
                }
            }
        }

        /// <summary>
        /// 获取下一步节点列表，伴随运行时条件信息
        /// </summary>
        /// <param name="currentActivity"></param>
        /// <param name="conditionKeyValuePair"></param>
        /// <returns></returns>
        internal NextActivityMatchedResult GetNextActivityList(Guid currentActivityGUID,
            IDictionary<string, string> conditionKeyValuePair = null)
        {
            try
            {
                NextActivityMatchedResult result = null;
                NextActivityMatchedType resultType = NextActivityMatchedType.Unknown;

                //创建“下一步节点”的根节点
                NextActivityComponent root = NextActivityComponentFactory.CreateNextActivityComponent();
                NextActivityComponent child = null;
                List<TransitionEntity> transitionList = GetForwardTransitionList(currentActivityGUID,
                    conditionKeyValuePair).ToList();

                if (transitionList.Count > 0)
                {
                    //遍历连线，获取下一步节点的列表
                    foreach (TransitionEntity transition in transitionList)
                    {
                        if (transition.ToActivity.ActivityType == ActivityTypeEnum.EndNode
                            || transition.ToActivity.ActivityType == ActivityTypeEnum.TaskNode
                            || transition.ToActivity.ActivityType == ActivityTypeEnum.SubProcessNode)
                        {
                            child = NextActivityComponentFactory.CreateNextActivityComponent(transition, transition.ToActivity);
                        }
                        else if (transition.ToActivity.ActivityType == ActivityTypeEnum.GatewayNode)
                        {
                            NextActivityScheduleBase activitySchedule = NextActivityScheduleFactory.CreateActivitySchedule(this,
                                transition.ToActivity.GatewaySplitJoinType);

                            child = activitySchedule.GetNextActivityListFromGateway(transition,
                                transition.ToActivity,
                                conditionKeyValuePair,
                                out resultType);
                        }
                        else
                        {
                            throw new XmlDefinitionException(string.Format("未知的节点类型：{0}", transition.ToActivity.ActivityType.ToString()));
                        }

                        if (child != null)
                        {
                            root.Add(child);
                            resultType = NextActivityMatchedType.Successed;
                        }
                    }
                }
                else
                {
                    resultType = NextActivityMatchedType.NoneTransitionFilteredByCondition;
                }
                result = NextActivityMatchedResult.CreateNextActivityMatchedResultObject(resultType, root);
                return result;
            }
            catch (System.Exception e)
            {
                throw new WfXpdlException(string.Format("解析流程定义文件发生异常，异常描述：{0}", e.Message), e);
            }
        }

        /// <summary>
        /// 获取下一步节点列表（伴随条件和资源）
        /// </summary>
        /// <param name="currentActivity"></param>
        /// <param name="conditionKeyValuePair"></param>
        /// <returns></returns>
        internal NextActivityMatchedResult GetNextActivityList(Guid currentActivityGUID,
            IDictionary<string, string> conditionKeyValuePair,
            ActivityResource activityResource,
            Expression<Func<ActivityResource, ActivityEntity, bool>> expression)
        {
            NextActivityComponent newRoot = NextActivityComponentFactory.CreateNextActivityComponent();

            //先获取未加运行时表达式过滤的下一步节点列表
            NextActivityMatchedResult result = GetNextActivityList(currentActivityGUID,
                conditionKeyValuePair);

            foreach (NextActivityComponent c in result.Root)
            {
                if (c.HasChildren)
                {
                    NextActivityComponent child = GetNextActivityListByExpressionRecurisivly(c, activityResource, expression);
                    if (child != null)
                    {
                        newRoot.Add(child);
                    }
                }
                else
                {
                    if (expression.Compile().Invoke(activityResource, c.Activity))
                    {
                        newRoot.Add(c);
                    }
                }
            }

            NextActivityMatchedResult newResult = null;
            if (newRoot.HasChildren)
            {
                newResult = NextActivityMatchedResult.CreateNextActivityMatchedResultObject(result.MatchedType, newRoot);
            }
            else
            {
                newResult = NextActivityMatchedResult.CreateNextActivityMatchedResultObject(NextActivityMatchedType.NoneTransitionFilteredByCondition,
                    newRoot);
            }
            return newResult;
        }

        /// <summary>
        /// 递归获取满足条件的下一步节点列表
        /// </summary>
        /// <param name="root"></param>
        /// <param name="activityResource"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        private NextActivityComponent GetNextActivityListByExpressionRecurisivly(NextActivityComponent root,
           ActivityResource activityResource,
           Expression<Func<ActivityResource, ActivityEntity, bool>> expression)
        {
            NextActivityComponent r1 = null;
            foreach (NextActivityComponent c in root)
            {
                if (c.HasChildren)
                {
                    NextActivityComponent child = GetNextActivityListByExpressionRecurisivly(c, activityResource, expression);
                    r1 = AddChildToNewGatewayComponent(r1, c, child);
                }
                else
                {
                    if (expression.Compile().Invoke(activityResource, c.Activity))
                    {
                        r1 = AddChildToNewGatewayComponent(r1, root, c);
                    }
                }
            }
            return r1;
        }

        /// <summary>
        /// 添加子节点到网关节点
        /// </summary>
        /// <param name="newRoot"></param>
        /// <param name="root"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        private NextActivityComponent AddChildToNewGatewayComponent(NextActivityComponent newRoot,
            NextActivityComponent root,
            NextActivityComponent child)
        {
            if ((newRoot == null) && (child != null))
                newRoot = NextActivityComponentFactory.CreateNextActivityComponent(root);

            if ((newRoot != null) && (child != null))
                newRoot.Add(child);
            return newRoot;
        }
        #endregion

        #endregion

        #region Xml活动节点操作
        /// <summary>
        /// 获取XML的节点信息
        /// </summary>
        /// <param name="activityGUID"></param>
        /// <returns></returns>
        private XmlNode GetXmlActivityNodeFromXmlFile(Guid activityGUID)
        {
            XmlNode xmlNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
                    string.Format("{0}[@id='" + activityGUID + "']", XPDLDefinition.StrXmlActivityPath));
            return xmlNode;
        }

        /// <summary>
        /// 获取活动节点的类型信息
        /// </summary>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        private XmlNode GetXmlActivityTypeNodeFromXmlFile(string nodeType)
        {
            XmlNode typeNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
                string.Format("{0}/ActivityType[@type='" + nodeType + "']", XPDLDefinition.StrXmlActivityPath));
            return typeNode;
        }

        /// <summary>
        /// 获取参与者信息
        /// </summary>
        /// <param name="participantGUID"></param>
        /// <returns></returns>
        private XmlNode GetXmlParticipantNodeFromXmlFile(Guid participantGUID)
        {
            XmlNode participantNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
                string.Format("{0}[@id='" + participantGUID + "']", XPDLDefinition.StrXmlSingleParticipantPath));
            return participantNode;
        }

        /// <summary>
        /// 获取当前节点信息
        /// </summary>
        /// <returns></returns>
        internal ActivityEntity GetActivity(Guid activityGUID)
        {
            XmlNode activityNode = GetXmlActivityNodeFromXmlFile(activityGUID);

            ActivityEntity entity = ConvertXmlActivityNodeToActivityEntity(activityNode);
            return entity;
        }

        /// <summary>
        /// 获取转移上的To节点的对象
        /// </summary>
        /// <param name="transitionNode">转移的xml节点</param>
        /// <returns></returns>
        private ActivityEntity GetActivityFromTransitionTo(XmlNode transitionNode)
        {
            string nextActivityGuid = XMLHelper.GetXmlAttribute(transitionNode, "to");
            XmlNode activityNode = GetXmlActivityNodeFromXmlFile(Guid.Parse(nextActivityGuid));

            ActivityEntity entity = ConvertXmlActivityNodeToActivityEntity(activityNode);
            return entity;
        }
        #endregion
        
        #region 获取节点上的角色信息
        /// <summary>
        /// 获取角色编码信息
        /// </summary>
        /// <param name="performerGUID"></param>
        /// <returns></returns>
        private int GetRoleIdFromXmlFile(Guid performerGUID)
        {
            XmlNode performerNode = GetXmlParticipantNodeFromXmlFile(performerGUID);
            var roleId = XMLHelper.GetXmlAttribute(performerNode, "outerId");
            return int.Parse(roleId);
        }

        /// <summary>
        /// 获取节点上定义的角色code集合
        /// </summary>
        /// <param name="activityGUID"></param>
        /// <returns></returns>
        internal IList<int> GetActivityRoles(Guid activityGUID)
        {
            IList<int> roles = new List<int>();
            XmlNode xmlNode = GetXmlActivityNodeFromXmlFile(activityGUID);
            XmlNode performersNode = xmlNode.SelectSingleNode("Performers");
            
            if (performersNode != null)
            {
                foreach (XmlNode performer in performersNode.ChildNodes)
                {
                    string performerGUID = XMLHelper.GetXmlAttribute(performer, "id");
                    roles.Add(GetRoleIdFromXmlFile(Guid.Parse(performerGUID)));
                }
            }
            return roles;
        }
        #endregion

        #region 获取节点数据项信息
        /// <summary>
        /// 获取当前节点所要求的数据项，即数据项必须填写，才可以触发后续流程
        /// </summary>
        /// <param name="activityGUID"></param>
        /// <returns></returns>
        internal IList<string> GetActivityDataItemsRequired(Guid activityGUID)
        {
            XmlNode requiredNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
               string.Format("{0}/DataItemsRequired", XPDLDefinition.StrXmlActivityPath));

            IList<string> itemList = new List<string>();
            foreach (XmlNode dataItemNode in requiredNode.ChildNodes)
            {
                string dataItemID = XMLHelper.GetXmlAttribute(dataItemNode, "id");
                XmlNode srcDataItemNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
                    string.Format("{0}[@id='" + dataItemID + "']", XPDLDefinition.StrXmlSingleDataItems));

                string dataItemCode = XMLHelper.GetXmlAttribute(srcDataItemNode, "code");
                itemList.Add(dataItemCode);
            }
            return itemList;
        }
        #endregion

        #region Xml节点转换信息
        /// <summary>
        /// 把XML节点转换为ActivityEntity实体对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal ActivityEntity ConvertXmlActivityNodeToActivityEntity(XmlNode node)
        {
            ActivityEntity entity = new ActivityEntity();
            entity.ActivityName = XMLHelper.GetXmlAttribute(node, "name");
            entity.ActivityGUID = Guid.Parse(XMLHelper.GetXmlAttribute(node, "id"));
            entity.ProcessGUID = ProcessEntity.ProcessGUID;

            //节点类型信息
            XmlNode typeNode = node.SelectSingleNode("ActivityType");
            entity.ActivityType = (ActivityTypeEnum)Enum.Parse(typeof(ActivityTypeEnum), XMLHelper.GetXmlAttribute(typeNode, "type"));
            entity.ActivityTypeDetail = ConvertXmlNodeToActivityTypeDetail(typeNode);

            if (entity.ActivityType == ActivityTypeEnum.SubProcessNode)
            {
                //子流程节点
                var subProcessNode = new SubProcessNode(entity);
                subProcessNode.SubProcessGUID = Guid.Parse(XMLHelper.GetXmlAttribute(typeNode, "subid"));
                entity.Node = subProcessNode;
            }
                     
            //任务完成类型信息
            XmlNode implementNode = node.SelectSingleNode("Implement");
            if (implementNode != null)
            {
                entity.TaskImplementDetail = new TaskImplementDetail();
                entity.TaskImplementDetail.ImplementationType = (ImplementationTypeEnum)Enum.Parse(typeof(ImplementationTypeEnum), XMLHelper.GetXmlAttribute(implementNode, "type"));

                //完成类型的详细信息
                XmlNode contentNode = implementNode.SelectSingleNode("Content");
                if (contentNode != null)
                {
                    entity.TaskImplementDetail.Assembly = XMLHelper.GetXmlAttribute(contentNode, "assembly");
                    entity.TaskImplementDetail.Interface = XMLHelper.GetXmlAttribute(contentNode, "interface");
                    entity.TaskImplementDetail.Method = XMLHelper.GetXmlAttribute(contentNode, "method");
                    entity.TaskImplementDetail.Content = contentNode.InnerText;
                }
            }

            //节点的Split Join 类型
            string gatewaySplitJoinType = XMLHelper.GetXmlAttribute(typeNode, "gatewaySplitJoinType");
            if (!string.IsNullOrEmpty(gatewaySplitJoinType))
            {
                entity.GatewaySplitJoinType = (GatewaySplitJoinTypeEnum)Enum.Parse(typeof(GatewaySplitJoinTypeEnum), gatewaySplitJoinType);
            }

            string gatewayDirection = XMLHelper.GetXmlAttribute(typeNode, "gatewayDirection");
            //节点的路由信息
            if (!string.IsNullOrEmpty(gatewayDirection))
            {
                entity.GatewayDirectionType = (GatewayDirectionEnum)Enum.Parse(typeof(GatewayDirectionEnum), gatewayDirection);
            }

            return entity;
        }

        /// <summary>
        /// 把Xml节点转换为ActivityTypeDetail 类（用于会签等复杂类型）
        /// </summary>
        /// <param name="typeNode"></param>
        /// <returns></returns>
        private ActivityTypeDetail ConvertXmlNodeToActivityTypeDetail(XmlNode typeNode)
        {
            ActivityTypeDetail entity = new ActivityTypeDetail();
            entity.ActivityType = (ActivityTypeEnum)Enum.Parse(typeof(ActivityTypeEnum), XMLHelper.GetXmlAttribute(typeNode, "type"));
            if (!string.IsNullOrEmpty(XMLHelper.GetXmlAttribute(typeNode, "complextype")))
            {
                entity.ComplexType = (ComplexTypeEnum)Enum.Parse(typeof(ComplexTypeEnum), XMLHelper.GetXmlAttribute(typeNode, "complextype"));
            }

            if (!string.IsNullOrEmpty(XMLHelper.GetXmlAttribute(typeNode, "mergetype")))
            {
                entity.MergeType = (MergeTypeEnum)Enum.Parse(typeof(MergeTypeEnum), XMLHelper.GetXmlAttribute(typeNode, "mergetype"));
            }

            if (!string.IsNullOrEmpty(XMLHelper.GetXmlAttribute(typeNode, "completeorder")))
            {
                entity.CompleteOrder =  float.Parse(XMLHelper.GetXmlAttribute(typeNode, "completeorder"));
            }

            return entity;
        }
        #endregion

        #region 转移连线的获取方法
        /// <summary>
        /// 获取连线属性
        /// </summary>
        /// <param name="fromActivityGUID"></param>
        /// <returns></returns>
        internal TransitionEntity GetTransition(Guid transitionGUID)
        {
            if (transitionGUID == null)
            {
                throw new WfXpdlException("传人参数transitionGUID为空，无法查找Transition数据！");
            }

            XmlNode xmlTransitionNode = GetXmlTransitionNode(transitionGUID);
            TransitionEntity transition = ConvertXmlTransitionNodeToTransitionEntity(xmlTransitionNode);

            return transition;
        }

        internal XmlNode GetXmlTransitionNode(Guid transitionGUID)
        {
            XmlNode transitionNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
                string.Format("{0}[@id='" + transitionGUID.ToString() + "']", XPDLDefinition.StrXmlTransitionPath));

            return transitionNode;
        }

        /// <summary>
        /// 获取活动转移的To节点信息
        /// </summary>
        /// <param name="fromActivityGUID"></param>
        /// <returns></returns>
        internal XmlNode GetForwardXmlTransitionNode(Guid fromActivityGUID)
        {
            XmlNode transitionNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
                string.Format("{0}[@from='" + fromActivityGUID.ToString() + "']", XPDLDefinition.StrXmlTransitionPath));

            return transitionNode;
        }

        /// <summary>
        /// 获取活动转移的To节点列表
        /// </summary>
        /// <param name="fromActivityGUID"></param>
        /// <returns></returns>
        internal XmlNodeList GetForwardXmlTransitionNodeList(Guid fromActivityGUID)
        {
            XmlNodeList transitionNodeList = XMLHelper.GetXmlNodeListByXpath(XmlProcessDefinition,
                string.Format("{0}[@from='" + fromActivityGUID.ToString() + "']", XPDLDefinition.StrXmlTransitionPath));

            return transitionNodeList;
        }

        /// <summary>
        /// 获取节点的后续连线
        /// </summary>
        /// <param name="fromActivityGUID"></param>
        /// <returns></returns>
        internal TransitionEntity GetForwardTransition(Guid fromActivityGUID)
        {
            XmlNode xmlTransitionNode = GetForwardXmlTransitionNode(fromActivityGUID);
            TransitionEntity transition = ConvertXmlTransitionNodeToTransitionEntity(xmlTransitionNode);

            return transition;
        }

        /// <summary>
        /// 获取当前节点的后续连线的集合
        /// </summary>
        /// <param name="fromActivityGUID"></param>
        /// <returns></returns>
        internal IList<TransitionEntity> GetForwardTransitionList(Guid fromActivityGUID)
        {
            IList<TransitionEntity> transitionList = new List<TransitionEntity>();
            XmlNodeList transitionNodeList = GetForwardXmlTransitionNodeList(fromActivityGUID);
            foreach (XmlNode transitionNode in transitionNodeList)
            {
                TransitionEntity entity = ConvertXmlTransitionNodeToTransitionEntity(transitionNode);
                transitionList.Add(entity);
            }
            return transitionList;
        }

        /// <summary>
        /// 获取当前节点的后续连线的集合（使用条件过滤）
        /// </summary>
        /// <param name="fromActivityGUID"></param>
        /// <param name="conditionKeyValuePair"></param>
        /// <returns></returns>
        internal IList<TransitionEntity> GetForwardTransitionList(Guid fromActivityGUID,
            IDictionary<string, string> conditionKeyValuePair)
        {
            IList<TransitionEntity> transitionList = new List<TransitionEntity>();
            XmlNodeList transitionNodeList = GetForwardXmlTransitionNodeList(fromActivityGUID);
            foreach (XmlNode transitionNode in transitionNodeList)
            {
                TransitionEntity entity = ConvertXmlTransitionNodeToTransitionEntity(transitionNode);
                bool isValidTranstion = IsValidTransition(entity, conditionKeyValuePair);
                if (isValidTranstion)
                {
                    transitionList.Add(entity);
                }
            }
            return transitionList;
        }

        /// <summary>
        /// XOrSplit类型下的连线列表
        /// </summary>
        /// <param name="fromActivityGUID"></param>
        /// <param name="conditionKeyValuePair"></param>
        /// <returns></returns>
        internal IList<TransitionEntity> GetForwardTransitionListWithConditionXOrSplit(Guid fromActivityGUID,
            IDictionary<string, string> conditionKeyValuePair)
        {
            IList<TransitionEntity> transitionList = new List<TransitionEntity>();
            XmlNodeList transitionNodeList = GetForwardXmlTransitionNodeList(fromActivityGUID);
            foreach (XmlNode transitionNode in transitionNodeList)
            {
                TransitionEntity entity = ConvertXmlTransitionNodeToTransitionEntity(transitionNode);
                bool isValidTranstion = IsValidTransition(entity, conditionKeyValuePair);
                if (isValidTranstion)
                {
                    transitionList.Add(entity);
                    break;
                }
            }
            return transitionList;
        }


        /// <summary>
        /// 获取活动转移的节点信息
        /// </summary>
        /// <param name="fromActivityGUID"></param>
        /// <returns></returns>
        private XmlNode GetForwardXmlTransitionNode(Guid fromActivityGUID,
            string toActivityGUID)
        {
            XmlNode transitionNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
                string.Format("{0}[@from='" + fromActivityGUID + " and to='" + toActivityGUID + "']", XPDLDefinition.StrXmlTransitionPath));
            return transitionNode;
        }

        /// <summary>
        /// 获取前驱节点的列表
        /// </summary>
        /// <param name="toActivityGUID"></param>
        /// <returns></returns>
        internal XmlNodeList GetXmlBackwardTransitonNodeList(Guid toActivityGUID)
        {
            XmlNodeList transtionNodeList = XMLHelper.GetXmlNodeListByXpath(XmlProcessDefinition,
                string.Format("{0}[@to='" + toActivityGUID + "']", XPDLDefinition.StrXmlTransitionPath));
            return transtionNodeList;
        }

        /// <summary>
        /// 获取节点的前驱连线
        /// </summary>
        /// <param name="toActivityGUID"></param>
        /// <returns></returns>
        internal IList<TransitionEntity> GetBackwardTransitionList(Guid toActivityGUID)
        {
            XmlNodeList transitionNodeList = GetXmlBackwardTransitonNodeList(toActivityGUID);
            IList<TransitionEntity> transitionList = new List<TransitionEntity>();
            foreach (XmlNode transitionNode in transitionNodeList)
            {
                TransitionEntity transition = ConvertXmlTransitionNodeToTransitionEntity(transitionNode);
                transitionList.Add(transition);
            }
            return transitionList;
        }

        /// <summary>
        /// 获取节点的前驱节点列表(Lambda表达式)
        /// </summary>
        /// <param name="activityGUID"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal IList<TransitionEntity> GetBackwardTransitionList(Guid activityGUID,
            Expression<Func<TransitionEntity, bool>> expression)
        {
            IList<TransitionEntity> transitionList = GetBackwardTransitionList(activityGUID);
            return GetBackwardTransitionList(activityGUID, expression);
        }

        /// <summary>
        /// 获取节点的前驱节点列表(Lambda表达式)
        /// </summary>
        /// <param name="activityGUID"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal IList<TransitionEntity> GetBackwardTransitionList(IList<TransitionEntity> transitionList,
            Expression<Func<TransitionEntity, bool>> expression)
        {
            IList<TransitionEntity> newTransitionList = new List<TransitionEntity>();
            foreach (TransitionEntity transition in transitionList)
            {
                if (expression.Compile().Invoke(transition))
                {
                    newTransitionList.Add(transition);
                }
            }
            return newTransitionList;
        }

        /// <summary>
        /// 根据流程定义文件，获取带有条件的节点前驱连线列表，（带有条件，可以用Lambda表达式重构）
        /// </summary>
        /// <param name="toActivityGUID"></param>
        /// <returns></returns>
        internal IList<TransitionEntity> GetBackworkTransitionListWithCondition(Guid toActivityGUID)
        {
            return GetBackwardTransitionList(toActivityGUID,
                (t => t.Condition != null && !string.IsNullOrEmpty(t.Condition.ConditionText)));
        }

        /// <summary>
        /// 获取并行连线的，类型为必需类型
        /// </summary>
        /// <param name="toActivityGUID"></param>
        /// <returns></returns>
        internal IList<TransitionEntity> GetBackwardTransitionListNecessary(IList<TransitionEntity> transitionList)
        {
            return GetBackwardTransitionList(transitionList,
                (t => (t.GroupBehaviour.ParallelOption != null) && (t.GroupBehaviour.ParallelOption == ParallelOptionEnum.Necessary)));
        }

        /// <summary>
        /// 获取节点前驱连线上必须的Token数目
        /// </summary>
        /// <param name="toActivityGUID"></param>
        /// <returns></returns>
        internal int GetBackwardTransitionListNecessaryCount(Guid toActivityGUID)
        {
            IList<TransitionEntity> backwardList = GetBackwardTransitionList(toActivityGUID);
            IList<TransitionEntity> necBackwardList = GetBackwardTransitionListNecessary(backwardList);
            return necBackwardList.Count;
        }

        /// <summary>
        /// 获取节点前驱连线的数目
        /// </summary>
        /// <param name="toActivityGUID"></param>
        /// <returns></returns>
        internal int GetBackwardTransitionListCount(Guid toActivityGUID)
        {
            IList<TransitionEntity> backwardList = GetBackwardTransitionList(toActivityGUID);
            return backwardList.Count;
        }

        #region 解析条件表达式
        /// <summary>
        /// 用LINQ解析条件表达式
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="conditionKeyValuePair"></param>
        /// <returns></returns>
        private bool ParseCondition(TransitionEntity transition, IDictionary<string, string> conditionKeyValuePair)
        {
            string expression = transition.Condition.ConditionText;
            string expressionReplaced = ReplaceParameterToValue(expression, conditionKeyValuePair);

            Expression e = System.Linq.Dynamic.DynamicExpression.Parse(typeof(Boolean), expressionReplaced);
            LambdaExpression LE = Expression.Lambda(e);
            Func<bool> testMe = (Func<bool>)LE.Compile();
            bool result = testMe();

            return result;
        }

        /// <summary>
        /// 是否是满足条件的Transition，如果条件为空，默认是有效的。
        /// </summary>
        /// <param name="forwardTransition"></param>
        /// <param name="conditionKeyValuePair"></param>
        /// <returns></returns>
        internal bool IsValidTransition(TransitionEntity transition,
           IDictionary<string, string> conditionKeyValuePair)
        {
            bool isValid = false;

            if (transition.Condition != null && !string.IsNullOrEmpty(transition.Condition.ConditionText))
            {
                if (conditionKeyValuePair != null)
                {
                    isValid = ParseCondition(transition, conditionKeyValuePair);
                }
            }
            else
            {
                //流程节点上定义的条件为空，则认为连线是可到达的
                isValid = true;
            }
            return isValid;
        }

        /// <summary>
        /// 判断整个连线集合，是否满足条件
        /// </summary>
        /// <param name="transitionList"></param>
        /// <param name="conditionKeyValuePair"></param>
        /// <returns></returns>
        internal bool CheckAndSplitOccurrenceCondition(List<TransitionEntity> transitionList,
            IDictionary<string, string> conditionKeyValuePair)
        {
            bool isValidAll = true;
            foreach (TransitionEntity transition in transitionList)
            {
                //只有是必需验证的条件，采取检查
                if (transition.GroupBehaviour.ParallelOption == ParallelOptionEnum.Necessary)
                {
                    bool isVailid = IsValidTransition(transition, conditionKeyValuePair);
                    if (!isVailid)
                    {
                        isValidAll = false;
                        break;
                    }
                }
            }
            return isValidAll;
        }

        /// <summary>
        /// 取代条件表达式中的参数值
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="dictoinary"></param>
        /// <returns></returns>
        internal string ReplaceParameterToValue(string expression, IDictionary<string, string> dictoinary)
        {
            foreach (KeyValuePair<string, string> p in dictoinary)
            {
                if (!ExpressionParser.IsNumeric(p.Value))
                {
                    //字符串类型的变量处理，加上双引号。
                    string s = "\"" + p.Value + "\"";
                    expression = expression.Replace(p.Key, s);
                }
                else
                {
                    expression = expression.Replace(p.Key, p.Value);
                }
            }
            return expression;
        }
        #endregion

        #region Xml节点转换信息
        /// <summary>
        /// 把XML节点转换为ActivityEntity实体对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal TransitionEntity ConvertXmlTransitionNodeToTransitionEntity(XmlNode node)
        {
            //构造转移的基本属性
            TransitionEntity entity = new TransitionEntity();
            entity.Name = XMLHelper.GetXmlAttribute(node, "name");
            entity.TransitionGUID = Guid.Parse(XMLHelper.GetXmlAttribute(node, "id"));
            entity.FromActivityGUID = Guid.Parse(XMLHelper.GetXmlAttribute(node, "from"));
            entity.ToActivityGUID = Guid.Parse(XMLHelper.GetXmlAttribute(node, "to"));
            entity.DirectionType = (TransitionDirectionTypeEnum)Enum.Parse(typeof(TransitionDirectionTypeEnum),
                XMLHelper.GetXmlAttribute(node, "direction"));

            //构造活动节点的实体对象
            entity.FromActivity = GetActivity(entity.FromActivityGUID);
            entity.ToActivity = GetActivity(entity.ToActivityGUID);

            //构造转移的条件节点
            XmlNode conditionNode = node.SelectSingleNode("Condition");
            if (conditionNode != null)
            {
                entity.Condition = new ConditionEntity();
                if (!string.IsNullOrEmpty(XMLHelper.GetXmlAttribute(conditionNode, "type")))
                {
                    entity.Condition.ConditionType = (ConditionTypeEnum)Enum.Parse(typeof(ConditionTypeEnum),
                        XMLHelper.GetXmlAttribute(conditionNode, "type"));
                }

                if ((conditionNode.SelectSingleNode("ConditionText") != null)
                    && !string.IsNullOrEmpty(XMLHelper.GetXmlNodeValue(conditionNode, "ConditionText")))
                {
                    entity.Condition.ConditionText = XMLHelper.GetXmlNodeValue(conditionNode, "ConditionText");
                }
            }

            //构造转移的行为节点
            XmlNode groupBehaviourNode = node.SelectSingleNode("GroupBehaviour");
            if (groupBehaviourNode != null)
            {
                entity.GroupBehaviour = new GroupBehaviourEntity();
                if (!string.IsNullOrEmpty(XMLHelper.GetXmlAttribute(groupBehaviourNode, "priority")))
                {
                    entity.GroupBehaviour.Priority = short.Parse(XMLHelper.GetXmlAttribute(groupBehaviourNode, "priority"));
                }

                if (!string.IsNullOrEmpty(XMLHelper.GetXmlAttribute(groupBehaviourNode, "parallelOption")))
                {
                    entity.GroupBehaviour.ParallelOption = (ParallelOptionEnum)Enum.Parse(typeof(ParallelOptionEnum),
                        XMLHelper.GetXmlAttribute(groupBehaviourNode, "parallelOption"));
                }
            }
            return entity;
        }
        #endregion
        #endregion

        #region 获取角色可编辑的数据项列表
        /// <summary>
        /// 获取角色可以编辑的数据项列表
        /// </summary>
        /// <param name="roleCode"></param>
        /// <returns></returns>
        internal IList<string> GetRoleDataItems(string roleCode)
        {
            XmlNode participantNode = XMLHelper.GetXmlNodeByXpath(this.XmlProcessDefinition,
                string.Format("{0}[@code='" + roleCode + "']", XPDLDefinition.StrXmlSingleParticipantPath));
            string participantGUID = XMLHelper.GetXmlAttribute(participantNode, "id");

            XmlNode participantDataItemsNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
                string.Format("{0}[@id='" + participantGUID + "']", XPDLDefinition.StrXmlParticipantDataItemPermissions));

            IList<string> itemList = new List<string>();
            foreach (XmlNode dataItemNode in participantDataItemsNode.ChildNodes)
            {
                string dataItemID = XMLHelper.GetXmlAttribute(dataItemNode, "id");
                XmlNode srcDataItemNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
                    string.Format("{0}[@id='" + dataItemID + "']", XPDLDefinition.StrXmlSingleDataItems));

                string dataItemCode = XMLHelper.GetXmlAttribute(srcDataItemNode, "code");
                itemList.Add(dataItemCode);
            }
            return itemList;
        }
        #endregion
    }
}
