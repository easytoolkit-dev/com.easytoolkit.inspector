using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolkit.Core;
using EasyToolkit.Core.Textual;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Inspector.Attributes;
using UnityEngine;

namespace EasyToolkit.Inspector.Editor
{
    [PostProcessorPriority(PostProcessorPriorityLevel.Super - 1)]
    public class GroupElementPostProcessor : PostProcessor
    {
        private Dictionary<Attribute, bool> _processedAttributeCache;

        protected override void Process()
        {
            if (Element.Children == null)
            {
                CallNextProcessor();
                return;
            }

            int elementIndex = 0;
            do
            {
                ProcessImpl(ref elementIndex);
            } while (elementIndex < Element.Children.Count);

            CallNextProcessor();
        }

        private void ProcessImpl(ref int elementIndex)
        {
            // Clear the cache at the start of each ProcessImpl call
            _processedAttributeCache?.Clear();

            if (!TryFindNextElement(ref elementIndex, out var beginGroupAttributeInfo))
            {
                return;
            }

            var elementChild = Element.Children[elementIndex];

            var beginGroupAttribute = (GroupAttribute)beginGroupAttributeInfo.Attribute;
            var beginGroupAttributeType = beginGroupAttribute.GetType();

            var newGroupDefinition = InspectorElements.Configurator.Group()
                .WithGroupAttribute(beginGroupAttributeType)
                .WithAdditionalAttributes(beginGroupAttributeInfo.Attribute)
                .WithName(beginGroupAttribute.GroupName)
                .CreateDefinition();
            var newGroupElement = Element.SharedContext.Tree.ElementFactory.CreateGroupElement(newGroupDefinition);

            if (elementChild is ILogicalElement logicalElement)
            {
                newGroupElement.AssociatedElement = logicalElement;
            }

            var childrenToMove = new List<IElement> { elementChild };

            if (beginGroupAttributeInfo.Source != ElementAttributeSource.Type)
            {
                CollectAllGroupChildren(beginGroupAttribute, elementIndex + 1, childrenToMove);
            }

            Element.Children.Insert(elementIndex, newGroupElement);
            elementIndex++;

            Element.Request(() =>
            {
                newGroupElement.Update();
                foreach (var child in childrenToMove)
                {
                    newGroupElement.Children.Add(child);
                }
            });
        }

        /// <summary>
        /// Collects children from the current group and all subsequent groups
        /// with matching catalogue, name, and type.
        /// </summary>
        private void CollectAllGroupChildren(
            GroupAttribute beginGroupAttribute,
            int startIndex,
            List<IElement> result)
        {
            var beginGroupAttributeType = beginGroupAttribute.GetType();

            while (true)
            {
                if (beginGroupAttribute.EndAfterThisProperty)
                {
                    break;
                }

                var groupChildren = FindGroupChildren(beginGroupAttribute, startIndex);

                if (groupChildren.Count == 0)
                {
                    break;
                }

                result.AddRange(groupChildren);

                // Get the index of the last collected element
                var lastIndex = startIndex + groupChildren.Count;

                if (lastIndex < 0 || lastIndex >= Element.Children.Count - 1)
                {
                    break;
                }

                // Search for the next group with matching properties
                var nextIndex = lastIndex + 1;
                if (!TryFindNextMatchingGroup(
                        nextIndex,
                        beginGroupAttributeType,
                        beginGroupAttribute.GroupName,
                        beginGroupAttribute.GroupCatalogue,
                        out var nextGroupInfo))
                {
                    break;
                }

                // Update for next iteration
                beginGroupAttribute = (GroupAttribute)nextGroupInfo.Attribute;
                startIndex = nextIndex + 1;
            }
        }

        private List<IElement> FindGroupChildren(
            GroupAttribute beginGroupAttribute,
            int startIndex)
        {
            var beginGroupAttributeType = beginGroupAttribute.GetType();
            var groupName = beginGroupAttribute.GroupName;
            var groupCatalogue = beginGroupAttribute.GroupCatalogue;

            var result = new List<IElement>();
            var subGroupStack = new Stack<IElement>();

            for (int i = startIndex; i < Element.Children.Count; i++)
            {
                var child = Element.Children[i];

                var childGroupAttribute = (GroupAttribute)child.GetAttribute(beginGroupAttributeType);
                if (childGroupAttribute != null)
                {
                    var childGroupCatalogue = childGroupAttribute.GroupCatalogue;
                    var childGroupName = childGroupAttribute.GroupName;

                    var isSameGroup = childGroupName == groupName && childGroupCatalogue == groupCatalogue;

                    if (!isSameGroup)
                    {
                        var isSubGroup = groupCatalogue.IsNotNullOrEmpty() &&
                                         childGroupCatalogue.IsNotNullOrEmpty() &&
                                         childGroupCatalogue.Length > groupCatalogue.Length &&
                                         childGroupCatalogue.StartsWith(groupCatalogue) &&
                                         childGroupCatalogue[groupCatalogue.Length] == '/';

                        if (isSubGroup)
                        {
                            subGroupStack.Push(child);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                var childEndGroupAttribute = child.GetAttribute<EndGroupAttribute>();
                if (childEndGroupAttribute != null)
                {
                    if (subGroupStack.Count > 0)
                    {
                        subGroupStack.Pop();
                    }
                    else
                    {
                        break;
                    }
                }

                result.Add(child);
            }

            return result;
        }

        /// <summary>
        /// Finds the next group element with matching catalogue, name, and attribute type.
        /// </summary>
        private bool TryFindNextMatchingGroup(
            int startIndex,
            Type beginGroupAttributeType,
            string groupName,
            string groupCatalogue,
            out ElementAttributeInfo matchingGroupInfo)
        {
            matchingGroupInfo = null;

            for (int i = startIndex; i < Element.Children.Count; i++)
            {
                var child = Element.Children[i];
                var attributeInfo = child.GetAttributeInfo(beginGroupAttributeType);

                if (attributeInfo != null)
                {
                    var attr = (GroupAttribute)attributeInfo.Attribute;

                    if (attr.GroupName == groupName &&
                        attr.GroupCatalogue == groupCatalogue)
                    {
                        matchingGroupInfo = attributeInfo;
                        return true;
                    }
                }
            }

            return false;
        }

        private int i = 0;

        private bool TryFindNextElement(ref int elementIndex, out ElementAttributeInfo beginGroupAttributeInfo)
        {
            beginGroupAttributeInfo = null;

            for (; elementIndex < Element.Children.Count; elementIndex++)
            {
                var child = Element.Children[elementIndex];
                foreach (var attributeInfo in child.GetAttributeInfos())
                {
                    i++;
                    if (i > 100)
                    {
                        ;
                    }

                    var attributeType = attributeInfo.Attribute.GetType();
                    if (!attributeType.IsDerivedFrom<GroupAttribute>())
                    {
                        continue;
                    }

                    // NOTE: We must check all parent group elements (not just the immediate Element) to avoid infinite recursion.
                    // This is necessary when a child element has multiple GroupAttributes (e.g., GroupAttributeA and GroupAttributeB).
                    // The processing flow in such cases is:
                    //   1. Process GroupAttributeA → Create GroupA → child becomes GroupA's child
                    //   2. Process GroupAttributeB → Create GroupB → child becomes GroupB's child, GroupB becomes GroupA's child
                    //   3. On next iteration, Element is GroupB and child is checked for GroupAttributes
                    // Without checking parent groups, child's GroupAttributeA would be detected as "unprocessed"
                    // (since GroupB only has GroupAttributeB), causing another GroupA to be created → infinite recursion.
                    if (IsAttributeProcessedInParentGroups(attributeInfo))
                    {
                        continue;
                    }

                    beginGroupAttributeInfo = attributeInfo;
                    return true;
                }
            }

            return false;
        }

        // NOTE: Checks if the given GroupAttribute has already been processed by any parent GroupElement.
        // This prevents infinite recursion when elements have multiple GroupAttributes by walking up the
        // element tree to check if any ancestor GroupElement already has this attribute.
        //
        // Example scenario that would cause infinite recursion without this check:
        //   - child has GroupAttributeA and GroupAttributeB
        //   - GroupA created (contains GroupAttributeA)
        //   - GroupB created (contains GroupAttributeB), child becomes GroupB's child, GroupB becomes GroupA's child
        //   - On next iteration with Element=GroupB: child still has both GroupAttributeA and GroupAttributeB
        //   - Without parent check: GroupAttributeA appears "unprocessed" → creates duplicate GroupA → recursion
        //   - With parent check: Detects GroupA (parent) already has GroupAttributeA → correctly skips it
        private bool IsAttributeProcessedInParentGroups(ElementAttributeInfo attributeInfo)
        {
            var attribute = attributeInfo.Attribute;

            // Check cache first to avoid redundant parent traversals
            if (_processedAttributeCache != null &&
                _processedAttributeCache.TryGetValue(attribute, out var cachedResult))
            {
                return cachedResult;
            }

            // Initialize cache on first use
            _processedAttributeCache ??= new Dictionary<Attribute, bool>();

            // Walk up the parent chain to check if any parent group has this attribute
            var current = Element;
            bool found = false;
            while (current is IGroupElement groupElement)
            {
                if (groupElement.TryGetAttributeInfo(attribute, out _))
                {
                    found = true;
                    break;
                }

                current = groupElement.Parent;
            }

            // Cache the result for future lookups
            _processedAttributeCache[attribute] = found;
            return found;
        }
    }
}
