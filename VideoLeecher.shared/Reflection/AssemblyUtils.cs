using System;
using System.Reflection;

namespace VideoLeecher.shared.Reflection
{
    public class AssemblyUtils
    {
        #region ველები

        private static AssemblyUtils _instance;

        private string _product;
        private Version _version;


        #endregion ველები

        #region თვისებები

        public static AssemblyUtils Get
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AssemblyUtils();
                }

                return _instance;
            }
             

        }

        #endregion თვისებები

        #region მეთოდები

        public Version GetAssemblyVersion()
        {
            if (_version == null)
            {
                Assembly ass = Assembly.GetExecutingAssembly();

                if (ass == null)
                {
                    throw new ApplicationException("Executing assembly  is null!");
                }

                AssemblyFileVersionAttribute att = ass.GetCustomAttribute<AssemblyFileVersionAttribute>();

                if (att == null)
                {
                    throw new ApplicationException("Couldn't find attribute of type  '" + typeof(AssemblyFileVersionAttribute).FullName + "'!");
                }

                if(!Version.TryParse(att.Version, out _version))
                {
                    throw new ApplicationException("Error while parsing  assembly  file version!!!");
                }
            }


            return _version;
        }

        public string GetProductName()
        {

            if(string.IsNullOrEmpty(_product))
            {
                Assembly a = Assembly.GetExecutingAssembly();

                if (a == null)
                {
                    throw new ApplicationException("Executing assembly  is null!");
                }

                AssemblyProductAttribute att = a.GetCustomAttribute<AssemblyProductAttribute>();

                if (att == null)
                {
                    throw new ApplicationException("Couldn't not  find attribute of type '" + typeof(AssemblyProductAttribute).FullName + "'!");
                }

                _product = att.Product;
            }

            return _product;
        }

        #endregion მეთოდები




    }
}
