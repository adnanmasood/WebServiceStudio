using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace WebServiceStudio
{
    internal class NullableGenericProperty : ClassProperty
    {
        public NullableGenericProperty(Type[] possibleTypes, string name, object val) : base(possibleTypes, name, val)
        {
            //this.isNull = false;
            //this.val = val;
            //this.isNull = this.val == null;
            if (possibleTypes.Length == 2)
            {
                if (possibleTypes[1].FullName == "System.Int32")
                {
                    //possibleTypes[0] = new Nullable  <int>();
                }
                possibleTypes[1] = null;
            }
        }

        [RefreshProperties(RefreshProperties.All), Editor(typeof (DynamicEditor), typeof (UITypeEditor)),
         TypeConverter(typeof (DynamicConverter))]
        public object Value
        {
            get { return base.InternalValue; }
            set
            {
                base.InternalValue = value;
                base.TreeNode.Text = ToString();
            }
        }

        protected override void CreateChildren()
        {
        }

        public override object ReadChildren()
        {
            return Value;
        }

        public override string ToString()
        {
            string str = base.ToString();
            if (Value == null)
            {
                return str;
            }
            return (str + " = " + Value);
        }
    }
}