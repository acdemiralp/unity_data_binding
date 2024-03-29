﻿using System;
using System.Linq;
using System.Reflection;

namespace UnityEngine.DataBinding
{
    public class DataRef : MemberRef
    {
        private FieldInfo           _fieldInfo;

        private PropertyInfo        _propertyInfo;
        private MethodInfo          _propertyGetMethod;
        private MethodInfo          _propertySetMethod;
        private bool                _propertyIsWritable;
        private readonly object[]   _propertyArgumentArray = new object[1];

        public Type     Type
        {
            get { return _fieldInfo != null ? _fieldInfo.FieldType : _propertyInfo.PropertyType; }
        }
        public object   Value
        {
            get { return _fieldInfo != null ? GetFieldValue() : GetPropertyValue(); }
            set
            {
                if (_fieldInfo != null)
                    SetFieldValue(value);
                else
                    SetPropertyValue(value);
            }
        }

        public DataRef(object target, string memberName) : base(target)
        {
            var memberInfo = Target.GetType().GetMember(memberName, BindingFlags).FirstOrDefault();
            if(memberInfo == null)
                throw new ArgumentException("Target does not contain member: " + memberName, "memberName");

            if(memberInfo is FieldInfo)
                SetupField((FieldInfo)memberInfo);
            else if(memberInfo is PropertyInfo)
                SetupProperty((PropertyInfo)memberInfo);
            else
                throw new ArgumentException("Member " + memberName + " is not a field or property", "memberName");
        }

        public override string ToString()
        {
            var objectString = Target != null ? Target.ToString() : "[NULL]";

            var dataString = "";
            if (_fieldInfo != null)
                dataString += _fieldInfo.Name;
            else if (_propertyInfo != null)
                dataString += _propertyInfo.Name;
            else
                dataString += "[NULL]";

            return objectString + "." + dataString;
        }

        private void    SetupField          (FieldInfo      fieldInfo)
        {
            _fieldInfo = fieldInfo;
        }
        private void    SetupProperty       (PropertyInfo   propertyInfo)
        {
            _propertyInfo       = propertyInfo;
            _propertyGetMethod  = propertyInfo.GetGetMethod();
            _propertySetMethod  = propertyInfo.GetSetMethod();
            _propertyIsWritable = _propertySetMethod != null;
        }
        private object  GetFieldValue       ()
        {
            return _fieldInfo.GetValue(Target);
        }
        private void    SetFieldValue       (object value)
        {
            if (_fieldInfo.IsLiteral)
                return;
        
            var fieldType = _fieldInfo.FieldType;

            if(value == null || fieldType.IsAssignableFrom(value.GetType()))
                _fieldInfo.SetValue(Target, value);
            else
                _fieldInfo.SetValue(Target, Convert.ChangeType(value, fieldType)); 
        }
        private object  GetPropertyValue    ()
        {
            return _propertyGetMethod.Invoke(Target, null);
        }
        private void    SetPropertyValue    (object value)
        {
            if (!_propertyIsWritable)
                return;

            var propertyType = _propertyInfo.PropertyType;

            if (value == null || propertyType.IsAssignableFrom(value.GetType()))
                _propertyArgumentArray[0] = value;
            else
                _propertyArgumentArray[0] = Convert.ChangeType(value, propertyType);

            _propertySetMethod.Invoke(Target, _propertyArgumentArray);
        }
    }
}
