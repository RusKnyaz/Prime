using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Knyaz.Optimus;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Prime.Model;
using HtmlElement = Knyaz.Optimus.Dom.Elements.HtmlElement;

namespace Prime.DevTools
{
	public partial class DomTreeControl : UserControl
	{
		private Engine _engine;
		private Document _document;

		public DomTreeControl()
		{
			InitializeComponent();
			ShowAttributes = true;
		}

		public Engine Engine
		{
			get { return _engine; }
			set
			{
				if (_engine != null)
				{
					Document = null;
					_engine.DocumentChanged -= OnDocumentChanged;
				}
					
				_engine = value;
				if (_engine != null)
				{
					this.SafeBeginInvoke(() => treeView1.Nodes.Clear());
					Document = _engine.Document;
					_engine.DocumentChanged += OnDocumentChanged;
				}
			}
		}

		private void OnDocumentChanged()
		{
			Document = _engine.Document;
		}

		private Document Document
		{
			get { return _document; }
			set
			{
				if (ReferenceEquals(_document, value))
					return;
				
				if (_document != null)
				{
					_document.NodeInserted -= DocumentOnDomNodeInserted;
					_document.NodeRemoved -= DocumentOnNodeRemoved;
					this.SafeInvoke(() => { treeView1.Nodes.Clear(); });
				}
				
				_document = value;
				if (_document != null)
				{
					this.SafeInvoke(() =>
					{
						//show exist nodes;
						foreach (var node in _document.ChildNodes)
						{
							DocumentOnDomNodeInserted(node);
						}
					});
					
					_document.NodeInserted += DocumentOnDomNodeInserted;
					_document.NodeRemoved += DocumentOnNodeRemoved;
				}
			}
		}

		private void DocumentOnNodeRemoved(Node parent, Node node)
		{
			var nodeToremove = FindTreeNode(node);

			if (nodeToremove != null)
				this.SafeInvoke(() => nodeToremove.Parent.Nodes.Remove(nodeToremove));
		}

		private Dictionary<Node, TreeNode> _nodes = new Dictionary<Node, TreeNode>();
		private Node _selectedNode;

		//todo: the index on node inserted should be considered.
		private void DocumentOnDomNodeInserted(Node child)
		{
			var targetTreeNodeCollection = FindTreeNodeCollection(child.ParentNode);
			if (targetTreeNodeCollection == null)
				return;

			this.SafeBeginInvoke(() =>
				{
					var newTreeNode = CreateBranch(child);

					if (newTreeNode != null)
					{
						var idx = child.ParentNode.ChildNodes.IndexOf(child);
						targetTreeNodeCollection.Insert(idx, newTreeNode);
					}
				});
		}

		public bool ShowAttributes { get; set; }

		private TreeNode CreateBranch(Node node)
		{
			string name = null;

			var element = node as Element;
			if (element != null)
			{
				name = "<" + element.TagName;
				if(ShowAttributes)
					name += string.Join("", element.Attributes.Select(x => " " + x.Name + "=\"" + x.Value + "\""));
				name += ">";
			}
			else
			{
				var comment = node as Comment;
				if (comment != null)
				{
					name = "<!-- " + comment.Data.Substring(0, Math.Min(comment.Data.Length, 50)) + "-->";
				}
				else
				{
					var attr = node as Attr;
					if (attr != null)
					{
						name = attr.Name;
						if (attr.Value != null)
						{
							name += ": " + attr.Value;
						}
					}
					else
					{
						var text = node as Text;
						if (text != null)
						{
							name = "\"" + text.Data.Substring(0, Math.Min(text.Data.Length, 50)) + "\"";
						}
					}
				}
			}

			if (name == null)
				name = node.ToString();
				
			var treeNode = new TreeNode(name) {Tag = node};


			Node[] nodes;
			lock (node.OwnerDocument)
			{
				nodes = node.ChildNodes.ToArray();	
			}
			
			foreach (var child in nodes.Select(CreateBranch).Where(x => x!=null))
			{
				treeNode.Nodes.Add(child);
			}
			if(!_nodes.ContainsKey(node))
				_nodes.Add(node, treeNode);
			return treeNode;
		}

		private TreeNode FindTreeNode(Node node)
		{
			if (node == null)
				return null;

			TreeNode treeNode;
			return _nodes.TryGetValue(node, out treeNode) ? treeNode : null;
		}

		private TreeNodeCollection FindTreeNodeCollection(Node parent)
		{
			if (parent == _engine.Document)
				return treeView1.Nodes;

			TreeNode treeNode;
			if (_nodes.TryGetValue(parent, out treeNode))
				return treeNode.Nodes;

			return null;
		}

		public Node SelectedNode
		{
			get { return _selectedNode; }
			set
			{
				_selectedNode = value;
				treeView1.SelectedNode = FindTreeNode(value);
			}
		}

		public event Action<TreeNode> NodeSelected;

		private void OnTreeViewAfterSelect(object sender, TreeViewEventArgs e)
		{
			if (NodeSelected != null)
				NodeSelected(e.Node);
		}

		private void clickToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var node = treeView1.SelectedNode;
			if (node != null)
			{
				var elt = node.Tag as HtmlElement;
				if (elt != null)
				{
					Task.Run(() => elt.Click());
					return;
				}
			}

			MessageBox.Show("Nothing to click");
		}

		private void setAttributeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			/*var node = treeView1.SelectedNode;
			if (node == null)
				return;

			var elt = node.Tag as HtmlElement;
			if(elt == null)
				return;

			var dlg = new SetAttributeForm {Element = elt};
			dlg.Show();*/
		}


		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			MessageBox.Show("Not implemented");
		}
	}
}
