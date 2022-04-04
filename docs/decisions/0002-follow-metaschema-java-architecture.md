# Follow Architecture in `metaschema-java` for Design

Date: 2022-04-04

## Status

Proposed

## Context

To implement one or more libraries for .NET runtimes, we must choose an implementation strategy for monolithic or composable libraries: an OSCAL-specific library or a modular, composable library design to parse [Metaschema](https://github.com/usnistgov/metaschema/) into C# object models used by separate OSCAL model and OSCAL CLI libraries, respectively. 

## Decision

We decide to implement this project with the modular strategy.

## Consequences

We have two general strategies to choose from for this work: monolithic or modular.

### Monolithic (OSCAL-Only, JSON or XML)

#### Description

One approach is to build libraries that will only "parse OSCAL" and build objects directly. Instead of parsing and serializing Metaschema definitions, a monolithic library will use [the outputs of the Metaschema pipeline's schema generation, be it JSON Schema or XML Schema](https://github.com/usnistgov/OSCAL/blob/bfcd0082bd4c08e16c7d5f5515adc5c858e8308e/build/ci-cd/README.md#workflow), as inputs. The original "source definitions" of those models in Metaschema XML will be ignored. The developer will have to chose to implement code that defines object models one or both schemas separately.

At the time of writing, the majority of open-source tools publicly demonstrate and proprietary tool developers indicate they use this strategy.

#### Pros

- Many tools in many languages exists to build object models directly from JSON Schema or XML Schema.
- JSON Schema and XML are veteran schema definition standards with strong community buy-in and support in their respective JSON and XML communities.
- This implementation strategy is simple and straightforward.
- Developers do not need upfront knowledge of OSCAL information model relationships as the schema technologies demonstrate those relationships for them.

#### Cons

- This approach is simple, but not flexible as OSCAL models change.
- Both JSON Schema and XML Schema are powerful, but have limits to the information they can encode about advanced Metaschema features necessary for practical OSCAL use: such as `constraint`s.
- The OSCAL pipelines that generate JSON and XML Schema are not designed to emit schemas to be extensible or modified in place. Developer who wish to make custom adaptations will have resources to build tooling to managed derived schemas.

### Modular

#### Description

Another approach is the modular architecture designed and implemented by the NIST OSCAL team with the [`metaschema-java`](https://github.com/usnistgov/metaschema-java/) and [`oscal-cli`](https://github.com/usnistgov/oscal-cli) libraries, respectively. In the former, an independent framework for Metaschema is implemented with an object model (for the Java programming language) and XML parser that maps source Metaschema definitions in XML to concrete instances of that object model, with added utility functions. An OSCAL application uses these packages' Java classes as a dependency with interfaces and class inheritance, defining OSCAL-specific functionality as needed as a layer above the core Metaschema internals.

For more details on such an architecture, see [`DESIGN.md`](../DESIGN.md).

At the time of this writing, tools publicly demonstrating the use of this strategy (in Java) are the [`oscal-cli`](https://github.com/usnistgov/oscal-cli) tool and [the umbrella of EasyDynamics OSCAL tools](https://github.com/EasyDynamics/?q=oscal&type=all&language=&sort=).

#### Pros

- This approach is more complex, but flexible due to decoupled, modular sub-systems.
- With a fully adaptive objective model, C# code can implemented features of Metaschema, beyond what existing serialization and validation libraries implemented for general JSON, XML, and YAML use cases (i.e. Metaschema `constraint`s, Metapath, et cetera). 
- Indirection can be used minimally or maximally to add functionality without significant rewrite of upstream OSCAL's official Metaschema XML definitions; frequently changing those for the growing public user community for code bases such as this one is high-risk.

#### Cons

- Upfront design and implementation will require more resources (personnel and their time).
- Documentation around design and implementation will be more important for library maintenance and developers using the library due to increased complexity.
- More complexity will require more thorough and consistent use of tests.
- The multiple .NET runtimes are not as consistent across supported hardware platforms like those for Java, despite .NET Standard's existence to bridge between the Windows-only .NET Framework and the cross-platform .NET Core. [Minor inconsistencies around library support and maintenance still occur](https://web.archive.org/web/20220226000927/https://andrewlock.net/stop-lying-about-netstandard-2-support/). More testing around coverage for Windows-based .NET Framework and .NET Core cross-platform execution will be needed.