using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using AdvLib.Tests.Adv_V2;
using NUnit.Framework;

namespace AdvLib.TestApp
{
    public class NUnitTestRunner
    {
        private List<Type> m_TestTypes = new List<Type>();
        private int m_FailedTests = 0;
        private int m_TotalTests = 0;
        private int m_ExecutedTests = 0;

        public void RunTests(Action<int> startTestsCallback, Action<int, int> updateProgressCallback, Action<int> finishedTestsCallback)
        {
            m_TestTypes.Clear();
            m_TotalTests = 0;

            Type[] allTypesInTestAssembly = typeof(TestV2FileGeneration).Assembly.GetTypes();
            foreach (Type type in allTypesInTestAssembly)
            {
                if (type.GetCustomAttributes(typeof (TestFixtureAttribute), true).Any())
                {
                    m_TestTypes.Add(type);

                    MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);

                    foreach (MethodInfo method in methods)
                    {
                        if (method.GetCustomAttributes(typeof(TestAttribute), true).Any())
                        {
                            int numTestCases = method.GetCustomAttributes(typeof (TestCaseAttribute), true).Count();

                            if (numTestCases > 0)
                                m_TotalTests += numTestCases;
                            else
                                m_TotalTests++;
                        }
                    }
                }
            }

            startTestsCallback(m_TotalTests);
            m_FailedTests = 0;
            m_ExecutedTests = 0;

            foreach(Type testType in m_TestTypes)
            {
                RunTests(testType, updateProgressCallback);
            }

            finishedTestsCallback(m_FailedTests);
        }

        private void RunTests(Type testType, Action<int, int> updateProgressCallback)
        {
            try
            {
                var instance = Activator.CreateInstance(testType);

                MethodInfo[] methods = testType.GetMethods(BindingFlags.Instance | BindingFlags.Public);

                MethodInfo setupMethod = methods.SingleOrDefault(x => x.GetCustomAttributes(typeof(SetUpAttribute), true).Any());
                MethodInfo tearDownMethod = methods.SingleOrDefault(x => x.GetCustomAttributes(typeof(TearDownAttribute), true).Any());

                foreach (MethodInfo method in methods)
                {
                    if (method.GetCustomAttributes(typeof(TestAttribute), true).Any())
                    {
                        TestCaseAttribute[] testCases = (TestCaseAttribute[])method.GetCustomAttributes(typeof(TestCaseAttribute), true);

                        if (testCases.Length > 0)
                        {
                            foreach (var testCase in testCases)
                            {
                                var testParams = method.GetParameters();
                                object[] arguments = new object[testParams.Length];
                                for (int i = 0; i < testParams.Length; i++)
                                {
                                    var val = testCase.Arguments[i];
                                    if (val.GetType() != testParams[i].ParameterType)
                                        arguments[i] = Convert.ChangeType(val, testParams[i].ParameterType);
                                    else
                                        arguments[i] = val;
                                }

                                RunTestInternal(method, arguments, instance, setupMethod, tearDownMethod);

                                updateProgressCallback(m_ExecutedTests, m_FailedTests);
                            }
                        }
                        else
                        {
                            RunTestInternal(method, null, instance, setupMethod, tearDownMethod);

                            updateProgressCallback(m_ExecutedTests, m_FailedTests);
                        }
                    }
                }
                
            }
            catch (Exception)
            {
                m_FailedTests++;
            }
            
        }

        private void RunTestInternal(MethodInfo test, object[] arguments, object instance, MethodInfo setupMethod, MethodInfo tearDownMethod)
        {
            m_ExecutedTests++;

            try
            {
                bool setupFailed = false;

                if (setupMethod != null)
                {
                    try
                    {
                        setupMethod.Invoke(instance, arguments);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);
                        m_FailedTests++;
                        setupFailed = true;
                    }
                }

                if (!setupFailed)
                {
                    try
                    {
                        test.Invoke(instance, arguments);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);
                        m_FailedTests++;
                    }
                }
            }
            finally
            {
                if (tearDownMethod != null)
                {
                    try
                    {
                        tearDownMethod.Invoke(instance, arguments);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);
                    }
                }
            }
        }
    }
}
