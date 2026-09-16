# PlantUML class diagram syntax cheat sheet

## Skeleton

```plantuml
@startuml MyDiagram
skinparam classAttributeIconSize 0
hide empty members

' -- types --

' -- relationships --

@enduml
```

## Declaring types

```plantuml
class ClassName
abstract class AbstractName
interface IInterfaceName
enum EnumName
```

Members inline:

```plantuml
class Order {
  - id: Guid
  - lines: List<OrderLine>
  + Total(): Money
  + AddLine(item: Item, qty: int): void
}
```

Visibility prefixes: `+` public, `-` private, `#` protected, `~` package. Prefix a
member with `{static}` or `{abstract}` for modifiers.

## Relationships

| Syntax | Meaning |
|---|---|
| `A --|> B` | inheritance / extends (A is a B) |
| `A ..|> B` | interface implementation |
| `A --> B` | association / uses |
| `A --* B` | composition (B lifetime owned by A) |
| `A --o B` | aggregation (B can outlive A) |
| `A ..> B` | dependency |
| `A -- B` | plain link, no direction |

Add multiplicities and labels:

```plantuml
Order "1" *-- "1..*" OrderLine : contains
Customer "1" o-- "0..*" Order : places
```

## Packages / namespaces

```plantuml
package "Domain" {
  class Order
  class Customer
}
```

## Notes and stereotypes

```plantuml
class Order <<AggregateRoot>>
note right of Order
  Enforces invariants for
  order totals and status.
end note
```

## Layout hints

- `left to right direction` — horizontal layout instead of top-down.
- `Order -[hidden]-> Customer` — force ordering without drawing a link.
- Group related classes together in the source; PlantUML lays out roughly in
  declaration order.

## DDD-flavored conventions used in this repo

- Tag aggregate roots with `<<AggregateRoot>>`, entities with `<<Entity>>`,
  value objects with `<<ValueObject>>`.
- Use composition (`*--`) from an aggregate root to the entities/value objects
  it owns; use plain association (`-->`) for references to other aggregates
  (reference by ID, not object graph).
- Keep repository/service interfaces in a separate `package "Domain Services"`
  block so they read distinctly from the entity model.
