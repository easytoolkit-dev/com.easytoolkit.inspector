# 解析器系统 (Resolver System)

## 概述

解析器系统是 EasyToolkit.Inspector.Editor 的核心扩展机制，负责解析元素的结构、操作、属性等。通过 Resolver 模式，框架可以灵活地支持各种自定义类型和逻辑。

## 解析器层次结构

```
IResolver (基础接口)
├── IStructureResolver (结构解析器)
│   ├── 获取子元素定义
│   └── 分析元素结构
│
├── IValueOperationResolver (值操作解析器)
│   ├── 提供值操作接口
│   └── 处理值读写
│
├── ICollectionStructureResolver (集合结构解析器)
│   ├── 集合项定义
│   └── 集合结构分析
│
├── IDrawerChainResolver (绘制链解析器)
│   ├── 构建绘制器链
│   └── 解析属性到绘制器
│
├── IAttributeResolver (属性解析器)
│   ├── 解析自定义属性
│   └── 提供属性信息
│
└── IPostProcessorChainResolver (后处理链解析器)
    ├── 元素后处理
    └── 树结构后处理
```

## IResolver 基础接口

```csharp
public interface IResolver
{
    // 所有 Resolver 的基础接口
}
```

## IStructureResolver - 结构解析器

**位置**: [Core/Resolvers/IStructureResolver.cs](../Core/Resolvers/IStructureResolver.cs)

### 接口定义

```csharp
public interface IStructureResolver : IResolver
{
    IElementDefinition[] GetChildrenDefinitions();
}
```

### 职责

1. **解析逻辑子结构** - 为 LogicalElement 提供子元素定义
2. **分析元素结构** - 确定元素的层次结构
3. **返回定义数组** - 供 ElementCreator 创建子元素

### 结构解析流程

```
LogicalElement.Initialize()
    ↓
StructureResolverFactory.CreateResolver(element)
    ↓
IStructureResolver.GetChildrenDefinitions()
    ↓
返回 IElementDefinition[]
    ↓
ElementCreator.CreateElement()
    ↓
LogicalElement.LogicalChildren
```

### 实现类

#### 1. ValueStructureResolverBase

值结构解析器基类，处理单个值的结构。

**位置**: [UI/Resolvers/StructureResolver/ValueStructureResolverBase.cs](../UI/Resolvers/StructureResolver/ValueStructureResolverBase.cs)

**职责**:
- 处理单个值的结构
- 支持泛型类型
- 为值类型提供子元素定义

#### 2. CollectionStructureResolverBase

集合结构解析器基类，处理数组、列表等集合类型。

**位置**: [UI/Resolvers/StructureResolver/Collection/CollectionStructureResolver.cs](../UI/Resolvers/StructureResolver/Collection/CollectionStructureResolver.cs)

**职责**:
- 处理数组、列表等集合类型
- 管理集合项元素
- 提供集合项的定义

#### 3. ReadOnlyCollectionStructureResolver

只读集合结构解析器。

**位置**: [UI/Resolvers/StructureResolver/Collection/ReadOnlyCollectionStructureResolver.cs](../UI/Resolvers/StructureResolver/Collection/ReadOnlyCollectionStructureResolver.cs)

#### 4. UnityCollectionStructureResolver

Unity 序列化集合结构解析器。

**位置**: [UI/Resolvers/StructureResolver/Collection/UnityCollectionStructureResolver.cs](../UI/Resolvers/StructureResolver/Collection/UnityCollectionStructureResolver.cs)

#### 5. MethodStructureResolverBase

方法结构解析器基类。

**位置**: [UI/Resolvers/StructureResolver/MethodStructureResolverBase.cs](../UI/Resolvers/StructureResolver/MethodStructureResolverBase.cs)

**职责**:
- 处理方法元素
- 解析方法参数
- 为方法参数提供定义

### 相关文件

- **接口**: [Core/Resolvers/IStructureResolver.cs](../Core/Resolvers/IStructureResolver.cs)
- **静态工厂**: [Core/Factories/StructureResolverFactory.cs](../Core/Factories/StructureResolverFactory.cs)
- **默认解析器**: [UI/Resolvers/StructureResolver/Value/GenericValueStructureResolver.cs](../UI/Resolvers/StructureResolver/Value/GenericValueStructureResolver.cs)

## IValueOperationResolver - 值操作解析器

**位置**: [Core/Resolvers/IValueOperationResolver.cs](../Core/Resolvers/IValueOperationResolver.cs)

### 接口定义

```csharp
public interface IValueOperationResolver : IResolver
{
    IValueOperation GetOperation();
}
```

### 职责

1. **提供值操作接口** - 返回 IValueOperation
2. **处理值读写** - 通过 Operation 实际读写值

### 操作解析流程

```
ValueElement.Initialize()
    ↓
ValueOperationResolverFactory.CreateResolver(element)
    ↓
IValueOperationResolver.GetOperation()
    ↓
返回 IValueOperation
    ↓
创建 ValueEntry(IValueOperation)
    ↓
ValueElement.ValueEntry
```

### 实现类

根据 `AsUnityProperty` 标志选择不同的操作类型：

- **true**: 返回 UnityPropertyOperation
- **false**: 返回 MemberValueOperation

### 相关文件

- **接口**: [Core/Resolvers/IValueOperationResolver.cs](../Core/Resolvers/IValueOperationResolver.cs)
- **静态工厂**: [Core/Factories/ValueOperationResolverFactory.cs](../Core/Factories/ValueOperationResolverFactory.cs)
- **默认解析器**: [UI/Resolvers/OperationResolver/GenericValueOperationResolver.cs](../UI/Resolvers/OperationResolver/GenericValueOperationResolver.cs)

## IDrawerChainResolver - 绘制链解析器

**位置**: [Core/Resolvers/IDrawerChainResolver.cs](../Core/Resolvers/IDrawerChainResolver.cs)

### 接口定义

```csharp
public interface IDrawerChainResolver : IResolver
{
    DrawerChain GetDrawerChain();
}
```

### 职责

1. **构建绘制器链** - 为元素创建绘制器责任链
2. **解析属性到绘制器** - 根据属性信息选择绘制器

### 绘制链解析流程

```
Element.Initialize()
    ↓
DrawerChainResolverFactory.CreateResolver(element)
    ↓
IDrawerChainResolver.GetDrawerChain()
    ↓
    ├─→ 查找适用的 ValueDrawer
    ├─→ 查找适用的 AttributeDrawer
    └─→ 按 DrawerPriority 排序
         ↓
返回 DrawerChain
    ↓
Element.DrawerChain
```

### 绘制器选择规则

1. **ValueDrawer** - 根据值类型选择
2. **AttributeDrawer** - 根据自定义属性选择
3. **优先级排序** - DrawerPriorityAttribute

### 相关文件

- **接口**: [Core/Resolvers/IDrawerChainResolver.cs](../Core/Resolvers/IDrawerChainResolver.cs)
- **静态工厂**: [Core/Factories/DrawerChainResolverFactory.cs](../Core/Factories/DrawerChainResolverFactory.cs)
- **默认解析器**: [UI/Resolvers/DrawerChainResolver/DefaultDrawerChainResolver.cs](../UI/Resolvers/DrawerChainResolver/DefaultDrawerChainResolver.cs)

## IAttributeResolver - 属性解析器

**位置**: [Core/Resolvers/IAttributeResolver.cs](../Core/Resolvers/IAttributeResolver.cs)

### 接口定义

```csharp
public interface IAttributeResolver : IResolver
{
    // 解析自定义属性
}
```

### 职责

1. **解析自定义属性** - 获取元素的自定义属性信息
2. **提供属性信息** - 返回 ElementAttributeInfo

### 相关文件

- **接口**: [Core/Resolvers/IAttributeResolver.cs](../Core/Resolvers/IAttributeResolver.cs)
- **静态工厂**: [Core/Factories/AttributeResolverFactory.cs](../Core/Factories/AttributeResolverFactory.cs)
- **默认解析器**: [UI/Resolvers/AttributeResolver/DefaultAttributeResolver.cs](../UI/Resolvers/AttributeResolver/DefaultAttributeResolver.cs)

## IPostProcessorChainResolver - 后处理链解析器

**位置**: [Core/Resolvers/IPostProcessorChainResolver.cs](../Core/Resolvers/IPostProcessorChainResolver.cs)

### 接口定义

```csharp
public interface IPostProcessorChainResolver : IResolver
{
    PostProcessorChain GetPostProcessorChain();
}
```

### 职责

1. **构建后处理器链** - 为元素创建后处理器责任链
2. **元素后处理** - 在元素创建后执行后处理逻辑
3. **树结构后处理** - 修改树结构（如创建分组）

### 后处理链解析流程

```
Element.PostProcessIfNeeded()
    ↓
PostProcessorChainResolverFactory.CreateResolver(element)
    ↓
IPostProcessorChainResolver.GetPostProcessorChain()
    ↓
    ├─→ 查找适用的 PostProcessor
    └─→ 按 PostProcessorPriority 排序
         ↓
返回 PostProcessorChain
    ↓
PostProcessorChain.Process()
    ↓
    ├─→ GroupElementPostProcessor.Process()
    └─→ LogicalElementPostProcessor.Process()
```

### 相关文件

- **接口**: [Core/Resolvers/IPostProcessorChainResolver.cs](../Core/Resolvers/IPostProcessorChainResolver.cs)
- **静态工厂**: [Core/Factories/PostProcessorChainResolverFactory.cs](../Core/Factories/PostProcessorChainResolverFactory.cs)
- **默认解析器**: [UI/Resolvers/PostProcessorResolver/DefaultPostProcessorChainResolver.cs](../UI/Resolvers/PostProcessorResolver/DefaultPostProcessorChainResolver.cs)

## Resolver 优先级

使用 `ResolverPriorityAttribute` 控制解析器选择顺序：

```csharp
[ResolverPriority]
public class MyCustomResolver : SomeResolverBase
{
    // 优先级高的解析器会优先被选择
}
```

### ResolverPriorityAttribute

**位置**: [Core/Resolvers/ResolverPriorityAttribute.cs](../Core/Resolvers/ResolverPriorityAttribute.cs)

## Resolver 工厂

### Resolver 静态工厂

```csharp
public static class XxxResolverFactory
{
    public static IXxxResolver CreateResolver(IElement element);
}
```

### 具体工厂

- **StructureResolverFactory** - 结构解析器工厂
- **ValueOperationResolverFactory** - 值操作解析器工厂
- **DrawerChainResolverFactory** - 绘制链解析器工厂
- **AttributeResolverFactory** - 属性解析器工厂
- **PostProcessorChainResolverFactory** - 后处理链解析器工厂

### 工厂实现位置

- **StructureResolverFactory**: [Core/Factories/StructureResolverFactory.cs](../Core/Factories/StructureResolverFactory.cs)
- **ValueOperationResolverFactory**: [Core/Factories/ValueOperationResolverFactory.cs](../Core/Factories/ValueOperationResolverFactory.cs)
- **DrawerChainResolverFactory**: [Core/Factories/DrawerChainResolverFactory.cs](../Core/Factories/DrawerChainResolverFactory.cs)
- **AttributeResolverFactory**: [Core/Factories/AttributeResolverFactory.cs](../Core/Factories/AttributeResolverFactory.cs)
- **PostProcessorChainResolverFactory**: [Core/Factories/PostProcessorChainResolverFactory.cs](../Core/Factories/PostProcessorChainResolverFactory.cs)

## 创建自定义 Resolver

### 1. 结构解析器

```csharp
[ResolverPriority]
public class MyCustomStructureResolver : ValueStructureResolverBase<MyCustomType>
{
    protected override IElementDefinition[] GetChildrenDefinitions()
    {
        // 返回子元素定义
        return new IElementDefinition[]
        {
            // ... 子元素定义
        };
    }
}
```

### 2. 操作解析器

```csharp
[ResolverPriority]
public class MyCustomOperationResolver : ValueOperationResolverBase<MyCustomType>
{
    protected override IValueOperation GetOperation()
    {
        // 返回自定义操作实现
        return new MyCustomOperation();
    }
}
```

## 设计模式

### 策略模式 (Strategy Pattern)

- Resolver 系统是策略模式的典型应用
- 运行时选择具体的 Resolver 实现
- 通过优先级属性控制选择

### 依赖注入 (Dependency Injection)

- Resolver 工厂通过 IElementSharedContext 注入
- 元素通过 SharedContext 访问 Resolver 工厂

## 相关文档

- [元素系统](./02-ElementSystem.md) - LogicalChildren 的解析
- [值条目系统](./04-ValueEntrySystem.md) - ValueOperation 的解析
- [绘制器系统](./06-DrawerSystem.md) - DrawerChain 的解析
- [后处理器系统](./07-PostProcessorSystem.md) - PostProcessorChain 的解析

---

最后更新: 2026-05-07
