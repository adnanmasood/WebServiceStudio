using System.ComponentModel;
using System.Drawing.Design;

namespace WebServiceStudio
{
    internal class PrimitiveProperty : TreeNodeProperty
    {
        private object val;

        public PrimitiveProperty(string name, object val) : base(new[] {val.GetType()}, name)
        {
            this.val = val;
        }

        [Editor(typeof (DynamicEditor), typeof (UITypeEditor)), TypeConverter(typeof (DynamicConverter))]
        public object Value
        {
            get { return val; }
            set
            {
                val = value;
                base.TreeNode.Text = ToString();
            }
        }

        public override object ReadChildren()
        {
            return Value;
        }

        public override string ToString()
        {
            return string.Concat(new[] {Type.Name, " ", base.Name, " = ", Value});
        }
    }
}