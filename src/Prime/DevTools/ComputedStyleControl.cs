using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Knyaz.Optimus.Dom.Interfaces;

namespace Prime.DevTools
{
	public partial class ComputedStyleControl : UserControl
	{
		private ICssStyleDeclaration _style;

		public ComputedStyleControl()
		{
			InitializeComponent();
		}

		public ICssStyleDeclaration Style
		{
			get { return _style; }
			set
			{
				_style = value;

				propertyGrid1.SelectedObject = value == null ? null : new CssStyleAdapter(value);
			}
		}
	}


	public class CssStyleAdapter : ICustomTypeDescriptor
	{
		private readonly ICssStyleDeclaration _style;

		public CssStyleAdapter(ICssStyleDeclaration style)
		{
			_style = style;
		}

		public virtual AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public virtual String GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public virtual String GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public virtual TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public virtual EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public virtual PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public virtual Object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public virtual EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public virtual EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public virtual PropertyDescriptorCollection GetProperties()
		{
			return ((ICustomTypeDescriptor)this).GetProperties(null);
		}

		private static string[] Properties = {
			"color", "display", "float", "font-family", "font-size",
			"margin-top","margin-right", "margin-bottom", "margin-left", "padding", "height", "width"};

		public virtual PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			return new PropertyDescriptorCollection(Properties.Select(x => new StylePropertyDescriptor(x)).ToArray(), true);
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return _style;
		}
	}

	class StylePropertyDescriptor : PropertyDescriptor
	{
		private readonly string _propertyName;

		public StylePropertyDescriptor(string propertyName) : 
			base(propertyName, new Attribute[0])
		{
			_propertyName = propertyName;
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override object GetValue(object component)
		{
			return ((ICssStyleDeclaration) component).GetPropertyValue(_propertyName);
		}

		public override void ResetValue(object component)
		{
			throw new NotImplementedException();
		}

		public override void SetValue(object component, object value)
		{
			throw new NotImplementedException();
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

		public override Type ComponentType
		{
			get { return typeof (ICssStyleDeclaration); }
		}

		public override bool IsReadOnly
		{
			get { return true; }
		}

		public override Type PropertyType
		{
			get { return typeof (string); }
		}
	}
}
