using System.Windows.Forms;

namespace WebServiceStudio
{
    public class XmlTreeNode : TreeNode
    {
        public XmlTreeNode(string text, int startPos) : base(text)
        {
            this.StartPosition = startPos;
        }

        public int EndPosition { get; set; }

        public int StartPosition { get; set; }
    }
}