# Java.Interop Copilot Instructions

## Project Overview

**Java.Interop** is a .NET library that provides Java Native Interface (JNI) bindings for managed languages such as C#. It enables bidirectional interoperability between .NET's Common Language Runtime (CLR) and Java Virtual Machines (JVMs), allowing .NET code to invoke Java methods and Java code to call back into managed code.

**Key Purpose**: Bridge code running on .NET's CLR and code running on a Java VM without requiring one runtime to run on the other.

**Primary Use Cases**:
- .NET for Android development (successor to Xamarin.Android)
- Desktop Java interop scenarios  
- Binding Java libraries for .NET consumption
- Cross-platform Java integration

## Architecture & Core Concepts

### JNI (Java Native Interface)
- Industry-standard interface for Java-native code interaction
- Provides type-safe bindings using structs like `JniObjectReference` instead of raw `IntPtr`
- Supports both SafeHandle-based (safer) and IntPtr-based (faster) implementations
- Reference types: Local, Global, and WeakGlobal references with proper lifecycle management

### Type System & Marshaling
- **JavaObject**: Base class for managed wrappers of Java objects
- **JniPeerMembers**: Caches method and field IDs for efficient access
- **Value Marshaling**: Converts between Java and .NET types (e.g., `java.lang.String` ↔ `System.String`)
- **Exception Marshaling**: Translates Java exceptions to .NET exceptions

### Code Generation Pipeline
1. **API Description**: XML files describing Java APIs
2. **Generator Tool**: Converts API descriptions to C# binding code
3. **Java Callable Wrappers (JCWs)**: Java stubs for calling managed methods
4. **Marshal Methods**: Runtime-generated or pre-compiled bridging code

## Repository Structure

### Core Libraries (`src/`)
- **`Java.Interop/`**: Main JNI binding library with core types and runtime
- **`Java.Interop.Dynamic/`**: C# 4.0 `dynamic` provider for runtime method invocation
- **`Java.Interop.Export/`**: `[Export]` attribute support for exposing managed methods to Java
- **`Java.Runtime.Environment/`**: JVM loading and lifecycle management
- **`Java.Base/`**: Bindings for core Java types (`java.lang.*`, etc.)

### Code Generation Tools (`tools/`)
- **`generator/`**: Primary tool for generating C# bindings from Java API descriptions
- **`class-parse/`**: Parses Java `.class` files and generates API descriptions
- **`java-source-utils/`**: Utilities for processing Java source code
- **`jcw-gen/`**: Generates Java Callable Wrapper classes
- **`param-name-importer/`**: Imports parameter names from Java source

### Supporting Libraries
- **`Java.Interop.Tools.JavaSource/`**: Javadoc parsing and XML documentation conversion
- **`Java.Interop.Tools.Maven/`**: Maven project integration and dependency resolution
- **`Xamarin.Android.Tools.Bytecode/`**: Java bytecode analysis and processing
- **`Xamarin.SourceWriter/`**: Code generation utilities

### Testing (`tests/`)
- Unit tests for all major components
- Performance benchmarks (`Java.Interop-PerformanceTests/`)
- Integration tests with real JVM instances
- Generator tests with sample API descriptions

### Samples (`samples/`)
- **`Hello-Core/`**: Minimal JNI usage without object mapping
- **`Hello-Java.Base/`**: Using core Java type bindings
- **`Hello-NativeAOT*/`**: Ahead-of-time compilation scenarios

## Development Patterns & Conventions

### Naming Conventions
- **`Java*` prefix**: High-level types participating in cross-VM object reference semantics
- **`Jni*` prefix**: Low-level types that do NOT participate in automatic object lifecycle management
- **`*Marshaler` suffix**: Types responsible for converting between Java and .NET types

### Error Handling
- Java exceptions are automatically converted to .NET exceptions
- Use `JniEnvironment.Errors.ExceptionOccurred()` for manual exception checking
- Wrap JNI calls in `try`/`finally` blocks for proper resource cleanup

### Memory Management
- Local references: Automatically cleaned up by JVM
- Global references: Must be explicitly freed via `JniObjectReference.Dispose()`
- Use `using` statements or `try`/`finally` for proper cleanup

### Threading
- JNI environments are thread-local
- Use `JniEnvironment.Current` to access the current thread's JNI environment
- Java objects can be shared across threads with proper reference management

## Common Development Tasks

### Binding a Java Library
1. Generate API description XML using `class-parse` tool
2. Customize binding with metadata XML files (method signatures, parameter names, etc.)
3. Run `generator` tool to create C# binding assemblies
4. Handle any manual fixups for complex scenarios

### Adding New Marshalers
1. Implement `JniValueMarshaler<T>` for your type
2. Register with `JniRuntime.ValueManager.RegisterMarshaler<T>()`
3. Add `[JniValueMarshaler]` attribute to types needing custom marshaling

### Working with Javadoc
- Use `Java.Interop.Tools.JavaSource` for parsing Javadoc comments
- Convert to XML documentation comments for C# intellisense
- Handle `@param`, `@return`, `@throws`, and other Javadoc tags

### Performance Optimization
- Cache `JniPeerMembers` instances to avoid repeated JNI lookups
- Use `JniArgumentValue` structs for efficient parameter passing
- Consider AOT compilation for marshal methods in production

## Build System

### Prerequisites
- .NET 7+ SDK
- Java Development Kit (for compiling Java test classes)
- Platform-specific JVM libraries

### Build Commands
```bash
# Initialize submodules and prepare build
dotnet build -t:Prepare

# Build all projects
dotnet build

# Run specific tests  
dotnet test tests/Java.Interop-Tests/

# Build with specific configuration
dotnet build -c Release
```

### Configuration
- Use `Configuration.Override.props` for local build customization
- Set `$(JdkJvmPath)` to specify JVM library location
- Configure `$(JAVA_HOME)` for Java tooling

## Testing Guidelines

### Unit Testing
- Mock JVM interactions where possible using test doubles
- Use `TestJVM` project for integration tests requiring real JVM
- Test both success and error scenarios

### Performance Testing
- Use `Java.Interop-PerformanceTests` for benchmarking
- Compare against baseline Xamarin.Android performance
- Test both SafeHandle and IntPtr-based implementations

### Generator Testing
- Create minimal API XML for testing specific binding scenarios
- Verify generated C# code compiles and behaves correctly
- Test edge cases like generic types, nested classes, annotations

## Documentation Standards

### Code Comments
- Use XML documentation comments (`///`) for public APIs
- Document JNI interop behavior and threading requirements
- Include usage examples for complex scenarios

### API Documentation
- Javadoc comments are automatically converted to XML docs
- Maintain consistency between Java and C# documentation
- Document platform differences and limitations

## Known Limitations & Considerations

### Platform Support
- Primary focus on Android and desktop JVMs
- Some features may be Android-specific (e.g., certain marshal methods)
- Cross-platform JVM library loading differences

### Performance
- JNI calls have inherent overhead compared to native .NET calls
- SafeHandle usage provides safety at performance cost
- Consider batching operations to minimize JNI boundary crossings

### Compatibility
- JNI specification compatibility across different JVM implementations
- Android vs desktop JVM behavioral differences
- .NET Framework vs .NET Core/.NET 5+ differences

## Useful Resources

- [JNI Specification](http://docs.oracle.com/javase/8/docs/technotes/guides/jni/spec/jniTOC.html)
- [.NET for Android Documentation](https://learn.microsoft.com/en-us/dotnet/android/)
- [Android JNI Performance Guide](https://developer.android.com/training/articles/perf-jni)
- [Project Architecture Documentation](Documentation/Architecture.md)
- [Build Configuration Guide](Documentation/BuildConfiguration.md)

## Getting Help

- Review existing tests for usage patterns
- Check [GitHub Issues](https://github.com/dotnet/java-interop/issues) for known problems
- Consult the [.NET Discord](https://aka.ms/dotnet-discord) for community support
- Follow [Coding Guidelines](http://www.mono-project.com/community/contributing/coding-guidelines/) for contributions