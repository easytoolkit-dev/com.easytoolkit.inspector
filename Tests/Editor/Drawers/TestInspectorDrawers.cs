using System;
using System.Linq;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Inspector.Attributes;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace EasyToolkit.Inspector.Editor.Tests
{
    internal class TestInspectorDrawers
    {
        [EasyInspector]
        public class TestComponent : MonoBehaviour
        {
            public TestClass test;
            public TestClass2 test2;
        }

        [Serializable]
        public class TestClass
        {

        }

        [Serializable]
        public class TestClass2
        {

        }

        public class TestClassEasyDrawer : EasyValueDrawer<TestClass>
        {
        }

        [CustomPropertyDrawer(typeof(TestClass))]
        public class TestClassUnityDrawer : PropertyDrawer
        {

        }

        [CustomPropertyDrawer(typeof(TestClass2))]
        public class TestClass2UnityDrawer : PropertyDrawer
        {

        }


        [Test]
        public void Test()
        {
            var testInstance = new GameObject().AddComponent<TestComponent>();
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            tree.BeginDraw();
            tree.EndDraw();
            var element = tree.Root.LogicalChildren[0];

            var originalTypeMatchCandidates = HandlerUtility.TypeMatcher.TypeMatchCandidates.ToArray();
            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(new []
            {
                new TypeMatchCandidate(typeof(TestClassEasyDrawer), 0,
                    HandlerUtility.GetConstraints(typeof(TestClassEasyDrawer))),
                new TypeMatchCandidate(typeof(UnityPropertyDrawer<>), 1,
                    HandlerUtility.GetConstraints(typeof(UnityPropertyDrawer<>)))
            });

            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(element).ToArray();
            Assert.AreEqual(1, drawerTypes.Length);
            Assert.AreEqual(typeof(TestClassEasyDrawer), drawerTypes[0]);

            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(originalTypeMatchCandidates);
        }


        [Test]
        public void Test2()
        {
            var testInstance = new GameObject().AddComponent<TestComponent>();
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            tree.BeginDraw();
            tree.EndDraw();
            var element = tree.Root.LogicalChildren[1];

            var originalTypeMatchCandidates = HandlerUtility.TypeMatcher.TypeMatchCandidates.ToArray();
            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(new []
            {
                new TypeMatchCandidate(typeof(GenericValueDrawer<>), 0,
                    HandlerUtility.GetConstraints(typeof(GenericValueDrawer<>))),
                new TypeMatchCandidate(typeof(UnityPropertyDrawer<>), 1,
                    HandlerUtility.GetConstraints(typeof(UnityPropertyDrawer<>)))
            });

            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(element).ToArray();
            Assert.AreEqual(2, drawerTypes.Length);
            Assert.AreEqual(typeof(UnityPropertyDrawer<TestClass2>), drawerTypes[0]);

            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(originalTypeMatchCandidates);
        }
    }
}
