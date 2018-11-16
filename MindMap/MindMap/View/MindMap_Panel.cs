﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MindMap.View;
using System.Reflection;

namespace MindMap.View
{
    public partial class MindMap_Panel : UserControl
    {
        public MindMap_Panel()
        {
            InitializeComponent();
        }

        private TreeNode g_BaseNode = null;

        private Font _TextFont = new Font(new FontFamily("微软雅黑"), 12);
        public Font TextFont
        {
            get
            {
                return _TextFont;
            }
            set
            {
                if (value == null) return;
                _TextFont = value;
                
                mindMapNode.SetTextFont(_TextFont);




            }
        }


        

        /// <summary> 为控件设置数据源
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="DataSource"></param>
        /// <param name="NodeStruct"></param>
        public void SetDataSource<T>(List<T> DataSource, TreeViewNodeStruct NodeStruct)
        {
            //制作TreeNode
            g_BaseNode = null;
            SetTreeNode<T>(DataSource, NodeStruct);
            mindMapNode.TextFont = _TextFont;
            mindMapNode.TreeNode = g_BaseNode;
        }

        /// <summary> 用递归的方式将List数据按树状图添加到指定节点下
        /// </summary>
        /// <typeparam name="T">要添加的数据的模型类</typeparam>
        /// <param name="DataSource">要添加的List数据</param>
        /// <param name="NodeStruct">List的结构</param>
        /// <param name="ParenNode">父节点[为空则添加到根节点]</param>
        private void SetTreeNode<T>(List<T> DataSource, TreeViewNodeStruct NodeStruct, TreeNode ParenNode = null)
        {
            PropertyInfo KeyProperty = typeof(T).GetProperty(NodeStruct.KeyName);
            PropertyInfo ValueProperty = typeof(T).GetProperty(NodeStruct.ValueName);
            PropertyInfo ParentProperty = typeof(T).GetProperty(NodeStruct.ParentName);
            List<T> CurrentAddList = null;
            if (ParenNode == null) CurrentAddList = DataSource.Where(T1 => string.IsNullOrEmpty(ParentProperty.GetValue(T1).ToString())).ToList();//没有父节点就取父节点为空的记录
            else CurrentAddList = DataSource.Where(T1 => ParentProperty.GetValue(T1).ToString() == ParenNode.Name).ToList();//有父节点就取ParentID为父节点的记录
            foreach (T item in CurrentAddList)//遍历取出的记录
            {
                string StrKey = KeyProperty.GetValue(item).ToString();
                string StrValue = ValueProperty.GetValue(item).ToString();
                string StrParentValue = ParentProperty.GetValue(item).ToString();
                TreeNode NodeTemp = new TreeNode() { Name = StrKey, Text = StrValue, ImageKey = StrKey, SelectedImageKey = StrKey };
                SetTreeNode<T>(DataSource, NodeStruct, NodeTemp);
                (ParenNode == null ? GetBaseNodes() : ParenNode.Nodes).Add(NodeTemp);//将取出的记录添加到父节点下，没有父节点就添加到控件的Nodes下
            }
        }

        private TreeNodeCollection GetBaseNodes()
        {
            if (g_BaseNode == null)
            {
                g_BaseNode = new TreeNode();
                g_BaseNode.Text = "根节点";
                g_BaseNode.Name = "BaseNode";
            }
            return g_BaseNode.Nodes;
        }

        /// <summary> 用于指明SetDataSource的泛型类的结构
        /// [指明传入的哪个属性是ID，哪个属性是父ID，哪个属性是展示在前台的文本]
        /// </summary>
        public class TreeViewNodeStruct
        {
            /// <summary> 用于添加到TreeNode.Name中的属性名称
            /// 一般用于存ID值
            /// </summary>
            public string KeyName { get; set; }
            /// <summary> 用于展示到前台的属性名称
            /// [添加到TreeNode.Text中的值]
            /// </summary>
            public string ValueName { get; set; }
            /// <summary> 父ID的属性名称
            /// </summary>
            public string ParentName { get; set; }

        }

    }  
}