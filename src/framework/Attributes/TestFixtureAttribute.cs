// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using NUnit.Framework.Api;

namespace NUnit.Framework
{
	/// <example>
	/// [TestFixture]
	/// public class ExampleClass 
	/// {}
	/// </example>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
	public class TestFixtureAttribute : Attribute, IApplyToTest
	{
		private string description;

        private object[] arguments;
        private bool isIgnored;
        private string ignoreReason;

#if CLR_2_0 && !NUNITLITE
        private Type[] typeArgs;
        private bool argsSeparated;
#endif

        /// <summary>
        /// Default constructor
        /// </summary>
        public TestFixtureAttribute() : this( null ) { }
        
        /// <summary>
        /// Construct with a object[] representing a set of arguments. 
        /// In .NET 2.0, the arguments may later be separated into
        /// type arguments and constructor arguments.
        /// </summary>
        /// <param name="arguments"></param>
        public TestFixtureAttribute(params object[] arguments)
        {
            this.arguments = arguments == null
                ? new object[0]
                : arguments;
        }

		/// <summary>
		/// Descriptive text for this fixture
		/// </summary>
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

        /// <summary>
        /// The arguments originally provided to the attribute
        /// </summary>
        public object[] Arguments
        {
            get 
            {
#if CLR_2_0 && !NUNITLITE
                if (!argsSeparated)
                    SeparateArgs();
#endif
                return arguments; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TestFixtureAttribute"/> should be ignored.
        /// </summary>
        /// <value><c>true</c> if ignore; otherwise, <c>false</c>.</value>
        public bool Ignore
        {
            get { return isIgnored; }
            set { isIgnored = value; }
        }

        /// <summary>
        /// Gets or sets the ignore reason. May set Ignored as a side effect.
        /// </summary>
        /// <value>The ignore reason.</value>
        public string IgnoreReason
        {
            get { return ignoreReason; }
            set
            {
                ignoreReason = value;
                isIgnored = ignoreReason != null && ignoreReason != string.Empty;
            }
        }

#if CLR_2_0 && !NUNITLITE
        /// <summary>
        /// Get or set the type arguments. If not set
        /// explicitly, any leading arguments that are
        /// Types are taken as type arguments.
        /// </summary>
        public Type[] TypeArgs
        {
            get
            {
                if (!argsSeparated)
                    SeparateArgs();

                return typeArgs;
            }
            set 
            { 
                typeArgs = value;
                argsSeparated = true;
            }
        }

        private void SeparateArgs()
        {
            int cnt = 0;
            if (arguments != null)
            {
                foreach (object o in arguments)
                    if (o is Type) cnt++;
                    else break;

                typeArgs = new Type[cnt];
                for (int i = 0; i < cnt; i++)
                    typeArgs[i] = (Type)arguments[i];

                if (cnt > 0)
                {
                    object[] args = new object[arguments.Length - cnt];
                    for (int i = 0; i < args.Length; i++)
                        args[i] = arguments[cnt + i];

                    arguments = args;
                }
            }
            else
                typeArgs = new Type[0];

            argsSeparated = true;
        }
#endif

        #region IApplyToTest Members

        /// <summary>
        /// Modifies a test by adding a description, if not already set.
        /// </summary>
        /// <param name="test">The test to modify</param>
        public void ApplyToTest(ITest test)
        {
            if (!test.Properties.Contains("_DESCRIPTION") && description != null)
                test.Properties["_DESCRIPTION"] = description;
        }

        #endregion
    }
}