## Inspector Package Working Rules

- These instructions extend the repository-level `AGENTS.md`.
- This package owns Unity Inspector attributes, element-tree infrastructure, editor drawers, UI Toolkit visual builders and
  processors, resolver systems, post-processors, editor windows, and package-local inspector tests.
- Before making non-trivial changes in this package, read the relevant files under `Editor/Documentation~` to understand the
  current architecture and extension points.
- Do not manually create or edit Unity `.meta` files.
- Keep reusable Inspector package implementation inside this package. Do not place package code under the host project's
  `Assets/` folder.

## Documentation Map

- `Editor/Documentation~/00-README.md` summarizes the module relationships and links the detailed design documents.
- `Editor/Documentation~/01-Overview.md` explains the overall Inspector architecture, entry points, dependencies, and
  performance mechanisms.
- `Editor/Documentation~/02-ElementSystem.md` covers `IElement`, logical/value/group/root elements, lifecycle phases, and
  element messaging.
- `Editor/Documentation~/03-ElementTree.md` covers `ElementTree`, `ElementCreator`, tree creation, deferred requests,
  refresh, destruction, draw flow, and shared context.
- `Editor/Documentation~/04-ValueEntrySystem.md` covers `IValueEntry`, accessors, state, change handling, value operations,
  dirty tracking, and multi-object editing.
- `Editor/Documentation~/05-ResolverSystem.md` covers structure, value operation, drawer chain, attribute, visual, and
  post-processor resolver extension points.
- `Editor/Documentation~/06-DrawerSystem.md` covers IMGUI drawer chains, drawer priorities, `EasyDrawer`,
  `EasyValueDrawer<TValue>`, and `EasyAttributeDrawer<TAttribute>`.
- `Editor/Documentation~/07-PostProcessorSystem.md` covers post-processor chains, group creation, logical child processing,
  and safe tree mutation through `Request`.

## Package Boundaries

- Put runtime-safe attribute contracts, marker types, and backend options in `Runtime/`.
- Put all UnityEditor, IMGUI, UI Toolkit, reflection UI, resolver, drawer, visual builder, visual processor, editor window,
  persistent context, and inspector rendering code in `Editor/`.
- Put editor tests in `Tests/Editor/` and follow the existing package-local asmdef patterns.
- Keep public runtime attributes lightweight. They should describe inspector intent and avoid editor-only behavior.
- Do not reference `UnityEditor` from the runtime assembly.
- Keep dependencies reflected consistently in `package.json` and the relevant `.asmdef` files when dependency changes are
  intentional.

## Core Architecture

- Treat `ElementTreeFactory` as the public creation entry point for element trees.
- Treat `ElementTree` as the coordinator for update IDs, dirty value application, callback queues, root drawing, and
  safe deferred execution.
- Treat `ElementCreator` as the owner of element creation, tracking, and destruction.
- Treat definitions under `Editor/Core/Definitions/` as immutable metadata used to create elements.
- Treat elements under `Editor/Core/Elements/` as runtime tree nodes with lifecycle state, parent/child relationships,
  drawer chains, post-processing, and messaging.
- Use `Request` or the existing callback queue when modifying element tree structure during drawing, updating, or
  post-processing.
- Preserve the distinction between dynamic `Parent`/`Children` and logical `LogicalParent`/`LogicalChildren`.
- Keep `IGroupElement` behavior as post-processed structure. Grouping should be produced by post-processors from group
  attributes rather than hard-coded into unrelated element implementations.

## Value And Collection Handling

- Route value access through `IValueEntry`, `IValueAccessor`, `IValueState`, and `IValueChangeHandler`.
- Add type-specific read/write behavior through `IValueOperation` and value operation resolvers instead of special-casing
  inside drawers.
- Preserve multi-object editing behavior: `SmartValue` represents shared values, while indexed access reads or writes a
  specific target.
- Preserve dirty tracking and batched application of value changes through the element tree.
- For collections, extend collection operations, collection entries, and collection structure resolvers in their existing
  folders before introducing new collection mechanisms.

## Resolver, Drawer, Visual, And Post-Processor Extensions

- Add new structure parsing through `IStructureResolver` implementations under `Editor/UI/Resolvers/StructureResolver/`.
- Add new value operation selection through `IValueOperationResolver` implementations under
  `Editor/UI/Resolvers/OperationResolver/`.
- Add new IMGUI value rendering through `EasyValueDrawer<TValue>` and attribute rendering through
  `EasyAttributeDrawer<TAttribute>` under `Editor/UI/Drawers/`.
- Add UI Toolkit rendering through the existing `IVisualBuilder` and `IVisualProcessor` systems under
  `Editor/Core/Visuals/` and `Editor/UI/Visuals/`.
- Add tree reshaping behavior through `IPostProcessor` implementations under `Editor/UI/PostProcessors/`.
- Use the existing priority attributes for resolver, drawer, visual builder, visual processor, and post-processor ordering.
- When implementing a chain participant, call the next drawer or processor when the local behavior is additive instead of
  terminal.

## Attribute Features

- Define new public inspector attributes in `Runtime/Attributes/` with English XML documentation.
- Place drawing, validation, visibility, grouping, and callback behavior for attributes in Editor code.
- Keep attribute drawers focused on presentation or inspector interaction. Move reusable structural behavior into resolvers,
  operations, visual processors, or post-processors as appropriate.
- Prefer existing attribute categories and folder layout: `Behavior`, `Display`, `Layout`, `Validation`, `Group`, and
  `Core`.

## Tests And Verification

- Add or update tests in `Tests/Editor/` for editor-only behavior.
- For new attributes, cover both the runtime attribute contract and the editor behavior when practical.
- For element, resolver, value entry, drawer, visual, or post-processor changes, prefer focused tests near the existing
  `Tests/Editor/Core`, `Tests/Editor/Drawers`, and `Tests/Editor/InspectorAttributes` patterns.
- When package dependencies or asmdefs change, verify the affected Unity assemblies compile.
- If Unity test execution is unavailable, at least run the narrowest available compile or test command and report the gap.
