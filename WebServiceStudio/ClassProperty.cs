using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace WebServiceStudio
{
    internal class ClassProperty : TreeNodeProperty
    {
        private bool isNull;
        private object val;

        public ClassProperty(Type[] possibleTypes, string name, object val) : base(possibleTypes, name)
        {
            isNull = false;
            this.val = val;
            isNull = this.val == null;
        }

        internal object InternalValue
        {
            get { return (isNull ? null : val); }
            set
            {
                val = value;
                isNull = value == null;
            }
        }

        [RefreshProperties(RefreshProperties.All)]
        public bool IsNull
        {
            get { return isNull; }
            set
            {
                if (isNull != value)
                {
                    if (!value)
                    {
                        if (val == null)
                        {
                            val = CreateNewInstance(Type);
                        }
                        if (val == null)
                        {
                            MessageBox.Show("Not able to create an instance of " + Type.FullName);
                            value = true;
                        }
                    }
                    else
                    {
                        ReadChildren();
                    }
                    isNull = value;
                    CreateChildren();
                    base.TreeNode.Text = ToString();
                }
            }
        }

        public override Type Type
        {
            get { return ((InternalValue != null) ? InternalValue.GetType() : base.Type); }
            set
            {
                try
                {
                    if (Type != value)
                    {
                        InternalValue = CreateNewInstance(value);
                    }
                }
                catch
                {
                    InternalValue = null;
                }
            }
        }

        protected override void CreateChildren()
        {
            base.TreeNode.Nodes.Clear();
            if (OkayToCreateChildren())
            {
                foreach (PropertyInfo info in Type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    object val = info.GetValue(this.val, null);
                    if ((val == null) && IsInput())
                    {
                        val = CreateNewInstance(info.PropertyType);
                    }
                    CreateTreeNodeProperty(base.GetIncludedTypes(info.PropertyType), info.Name, val)
                        .RecreateSubtree(base.TreeNode);
                }
                foreach (FieldInfo info2 in Type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    object obj3 = info2.GetValue(val);

                    if ((obj3 == null) && IsInput())
                    {
                        obj3 = CreateNewInstance(info2.FieldType);
                    }
                    CreateTreeNodeProperty(base.GetIncludedTypes(info2.FieldType), info2.Name, obj3)
                        .RecreateSubtree(base.TreeNode);
                }
            }
        }

        protected virtual bool OkayToCreateChildren()
        {
            if (IsInternalType(Type))
            {
                return false;
            }
            if (IsDeepNesting(this))
            {
                InternalValue = null;
            }
            if (InternalValue == null)
            {
                return false;
            }
            return true;
        }

        public override object ReadChildren()
        {
            object internalValue = InternalValue;
            if (internalValue == null)
            {
                return null;
            }
            int num = 0;
            foreach (PropertyInfo info in Type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                TreeNode node = base.TreeNode.Nodes[num++];
                var tag = node.Tag as TreeNodeProperty;
                if (tag != null)
                {
                    info.SetValue(internalValue, tag.ReadChildren(), null);
                }
            }
            foreach (FieldInfo info2 in Type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                TreeNode node2 = base.TreeNode.Nodes[num++];
                var property2 = node2.Tag as TreeNodeProperty;
                if (property2 != null)
                {
                    info2.SetValue(internalValue, property2.ReadChildren(), BindingFlags.Public, null, null);
                }
            }
            return internalValue;
        }

        public virtual object ToObject()
        {
            return InternalValue;
        }

        public override string ToString()
        {
            return (base.GetTypeList()[0].Name + " " + base.Name + (IsNull ? " = null" : ""));
        }
    }
}