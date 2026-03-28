# .NET 10 Upgrade Plan

## Table of Contents

- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Project-by-Project Plans](#project-by-project-plans)
  - [SummonMyStrength.Api](#summonmystrengthapi)
  - [SummonMyStrength.Installer](#summonmystrengthinstaller)
  - [SummonMyStrength.Maui](#summonmystrengthmaui)
  - [SummonMyStrength.Simulation](#summonmystrengthsimulation)
- [Package Update Reference](#package-update-reference)
- [Breaking Changes Catalog](#breaking-changes-catalog)
- [Risk Management](#risk-management)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

---

## Executive Summary

### Scenario Description

This plan outlines the upgrade of the SummonMyStrength solution from .NET 8 and .NET Framework 4.7.2 to .NET 10.

### Scope

**Projects Affected:** 4 projects
- **SummonMyStrength.Api** (net8.0 → net10.0): Class library with 2856 LOC
- **SummonMyStrength.Installer** (net472 → net10.0-windows): WinForms installer with 45 LOC
- **SummonMyStrength.Maui** (net8.0-android/ios/maccatalyst/windows → add net10.0-windows): MAUI app with 2072 LOC
- **SummonMyStrength.Simulation** (net8.0 → net10.0): Class library with 272 LOC

**Current State:**
- Total: 5,245 lines of code across 184 files
- 9 NuGet packages (4 requiring updates)
- Mix of .NET 8 and .NET Framework 4.7.2

**Target State:**
- All projects targeting .NET 10 (or net10.0-windows for Windows-specific projects)
- All packages updated to compatible versions
- 0 compilation errors, 0 API compatibility issues

### Selected Strategy

**All-At-Once Strategy** - All projects upgraded simultaneously in single atomic operation.

**Rationale:**
- **Small solution**: 4 projects (threshold: <30 projects)
- **Simple dependencies**: Depth of 2 with clear structure (Api has no dependencies; Maui and Simulation depend on Api; Installer is standalone)
- **Low complexity**: All projects rated Low difficulty, total 28+ LOC to modify
- **Minimal risk**: No security vulnerabilities, no high-risk projects
- **Package clarity**: All 4 packages requiring updates have known .NET 10 compatible versions
- **Fast execution**: Atomic approach minimizes total timeline

### Discovered Metrics

| Metric | Value | Assessment |
|--------|-------|------------|
| Total Projects | 4 | Small solution |
| Dependency Depth | 2 | Simple structure |
| Circular Dependencies | 0 | Clean graph |
| High-Risk Projects | 0 | Low risk |
| Security Vulnerabilities | 0 | No critical issues |
| Package Updates Required | 4 | Manageable |
| Source Incompatible APIs | 10 | Minimal |
| Behavioral Changes | 18 | Requires testing |
| Estimated LOC to Modify | 28+ | 0.5% of codebase |

### Complexity Classification

**Classification: Simple**

Justification:
- Solution size: 4 projects (well below 5-project threshold)
- Dependency complexity: Maximum depth 2, no cycles
- Risk profile: All projects Low difficulty, no blocking issues
- Security: Zero vulnerabilities requiring immediate attention
- API compatibility: 99.5% compatible (5846/5874 APIs)

### Critical Issues

None. All identified issues are standard upgrade tasks:
- Framework version updates (all projects)
- Package version updates (4 packages)
- Source incompatible API fixes (10 instances, primarily System.Management and TimeSpan)
- Behavioral change validation (18 instances for runtime testing)

### Recommended Approach

**All-At-Once with single atomic upgrade phase:**
1. Update all project files simultaneously
2. Update all package references simultaneously  
3. Build entire solution and fix compilation errors
4. Validate with full solution build and tests

### Expected Remaining Iterations

- **Phase 2 Foundation**: 3 iterations (Dependency Analysis, Migration Strategy, Project Stubs & Risk)
- **Phase 3 Detail Generation**: 2 iterations (batch all projects, final sections)
- **Total**: 8 iterations including Phase 1 (3 completed)

---

## Migration Strategy

### Approach Selection

**Selected: All-At-Once Strategy**

All projects in the solution will be upgraded simultaneously in a single coordinated operation.

### Justification

**Why All-At-Once is Optimal:**

1. **Solution Size**: 4 projects (well below 30-project threshold for All-At-Once)
2. **Current Framework Uniformity**: 3 projects on .NET 8, 1 on .NET Framework 4.7.2 - all modern, SDK-style projects
3. **Dependency Simplicity**: Maximum depth of 2 with no circular dependencies
4. **Low Risk Profile**: All projects rated Low difficulty, no security vulnerabilities
5. **Package Compatibility**: All required package updates have clear .NET 10-compatible versions available
6. **Minimal Code Impact**: Only 28+ LOC estimated to modify (0.5% of codebase)
7. **Clean Assessment**: Zero binary incompatibilities, only 10 source incompatibilities (easily addressable)

**Advantages in This Context:**
- Fastest total completion time (single upgrade phase)
- No multi-targeting complexity or intermediate states
- All projects benefit from .NET 10 simultaneously
- Single testing cycle for entire solution
- Simplified coordination (one atomic change)

**Risk Mitigation:**
- Comprehensive build validation before compilation fixes
- Leveraging breaking changes catalog for known issues
- All changes in dedicated upgrade branch (`upgrade-to-NET10`)
- Single commit strategy for easy rollback if needed

### All-At-Once Strategy Rationale

The All-At-Once approach is specifically suitable because:
- Assessment shows all NuGet packages have known compatible versions for .NET 10
- No complex external dependency integration required
- Homogeneous codebase with consistent patterns (.NET MAUI and class libraries)
- Solution can be tested as a unit (MAUI app provides integration test surface)

### Dependency-Based Ordering

While All-At-Once updates all projects simultaneously, the build system will automatically respect dependencies:

**Implicit Build Order:**
1. **SummonMyStrength.Api** (0 dependencies) - builds first
2. **SummonMyStrength.Maui** (depends on Api) - builds after Api
3. **SummonMyStrength.Simulation** (depends on Api) - builds after Api
4. **SummonMyStrength.Installer** (0 dependencies) - builds independently

**File Update Order:** All project files and package references are updated before any build attempts, ensuring consistency.

### Execution Approach

**Sequential Operations (single atomic task):**

1. **Update Project Files** - Modify TargetFramework property in all .csproj files simultaneously
2. **Update Package References** - Update all PackageReference versions in all projects simultaneously
3. **Restore Dependencies** - Run `dotnet restore` for entire solution
4. **Build Solution** - Compile entire solution to identify errors
5. **Fix Compilation Errors** - Address breaking changes identified by build
6. **Rebuild & Verify** - Confirm solution builds with 0 errors

All steps are part of one coordinated operation with no intermediate commits.

### Phase Definitions

**Phase 0: Preparation** (if needed)
- Verify .NET 10 SDK installed
- Confirm on `upgrade-to-NET10` branch

**Phase 1: Atomic Upgrade**
- Update all projects to target framework
- Update all packages to compatible versions
- Fix all compilation errors
- Deliverable: Solution builds with 0 errors

**Phase 2: Validation**
- Run all tests (if test projects exist)
- Validate runtime behavior
- Deliverable: All tests pass, no regressions

---

## Detailed Dependency Analysis

### Dependency Graph Summary

The solution has a simple, clean dependency structure with no circular dependencies:

```
SummonMyStrength.Api (leaf - 0 dependencies)
    ↑
    ├── SummonMyStrength.Maui (depends on Api)
    └── SummonMyStrength.Simulation (depends on Api)

SummonMyStrength.Installer (standalone - 0 dependencies)
```

**Dependency Characteristics:**
- **Maximum depth**: 2 levels
- **Circular dependencies**: None
- **Leaf nodes**: 2 (Api, Installer)
- **Root nodes**: 2 (Maui, Simulation)

### Project Groupings by Migration Phase

Since this is an **All-At-Once** upgrade, all projects are upgraded simultaneously in a single phase:

**Phase 1: Atomic Upgrade (All Projects)**
1. **SummonMyStrength.Api** - Class library, 0 dependencies, 2 dependants
2. **SummonMyStrength.Installer** - WinForms installer, 0 dependencies, 0 dependants
3. **SummonMyStrength.Maui** - MAUI app, depends on Api
4. **SummonMyStrength.Simulation** - Class library, depends on Api

**Rationale for All-At-Once:**
- Small project count enables coordinated update
- Simple dependency structure (no complex chains)
- All projects currently on modern frameworks (.NET 8 or .NET Framework 4.7.2)
- Package updates are straightforward with known compatible versions
- No inter-project breaking changes expected

### Critical Path Identification

The critical path for build success:

1. **SummonMyStrength.Api** must build successfully (it has dependants)
2. **SummonMyStrength.Maui** and **SummonMyStrength.Simulation** must build after Api (they depend on it)
3. **SummonMyStrength.Installer** is independent and can build in any order

However, since we're using All-At-Once strategy, all projects are updated together and built as a complete solution. The dependency order is respected by the build system automatically.

**Build Validation Order:**
1. Solution-wide restore
2. Solution-wide build (build system respects dependency order)
3. Verify 0 errors across all projects

### Circular Dependencies

None identified. The dependency graph is acyclic.

---

## Project-by-Project Plans

### SummonMyStrength.Api

**Current State:**
- Target Framework: net8.0
- Project Type: Class Library (SDK-style)
- Dependencies: 0 project dependencies
- Dependants: 2 (Maui, Simulation)
- Files: 104 files, 2856 LOC
- Packages: 4 packages (all requiring updates)
- Risk Level: Low

**Target State:**
- Target Framework: net10.0
- Updated Packages: 4 packages to version 10.0.5

#### Migration Steps

**1. Prerequisites**
- Ensure .NET 10 SDK installed on build machine
- Verify SummonMyStrength.Api.csproj file is not locked

**2. Framework Update**
Change in `SummonMyStrength.Api/SummonMyStrength.Api.csproj`:
```xml
<TargetFramework>net10.0</TargetFramework>
```
(Previously: `net8.0`)

**3. Package Updates**

| Package | Current Version | Target Version | Reason |
|---------|----------------|----------------|--------|
| Microsoft.Extensions.DependencyInjection.Abstractions | 8.0.0 | 10.0.5 | Framework compatibility |
| Microsoft.Extensions.Http | 8.0.0 | 10.0.5 | Framework compatibility |
| System.Management | 5.0.0 | 10.0.5 | Framework compatibility + bug fixes |
| (Microsoft.Extensions.Logging.Abstractions) | (implicit) | (10.0.5) | Transitive dependency via Http package |

**4. Expected Breaking Changes**

Based on assessment, 8 source incompatible APIs and 18 behavioral changes:

**Source Incompatible (require code changes):**

- **System.TimeSpan.FromSeconds(double)** (2 instances)
  - Issue: Overload resolution changes in .NET 9+
  - Fix: Explicitly cast to `double` if ambiguous: `TimeSpan.FromSeconds((double)value)`

- **System.Security.Authentication.SslProtocols.Tls** (1 instance)
  - Issue: Obsolete in .NET 10, removed from default allowed protocols
  - Fix: Use `SslProtocols.Tls12` or `SslProtocols.Tls13` minimum

- **System.Security.Authentication.SslProtocols.Tls11** (1 instance)
  - Issue: Obsolete in .NET 10, removed from default allowed protocols
  - Fix: Use `SslProtocols.Tls12` or `SslProtocols.Tls13` minimum

- **System.Management APIs** (6 instances across ManagementObject, ManagementObjectSearcher, etc.)
  - Issue: Nullable reference type annotations added, may cause warnings
  - Fix: Update System.Management package to 10.0.5; add null checks where compiler indicates

**Behavioral Changes (require testing, may not need code changes):**

- **System.Net.Http.HttpContent** (6 instances)
  - Change: Disposal behavior and stream handling changes
  - Validation: Test HTTP client usage, ensure streams disposed correctly

- **System.Uri** (5 instances + constructor 3 instances)
  - Change: URI parsing strictness increased
  - Validation: Test URI parsing with edge cases, verify existing URIs still valid

- **System.Text.Json.JsonSerializer.Deserialize** (1 instance)
  - Change: Deserialization behavior for certain edge cases
  - Validation: Test JSON deserialization, especially with complex types

- **Microsoft.Extensions.DependencyInjection.HttpClientFactoryServiceCollectionExtensions.AddHttpClient** (2 instances)
  - Change: HttpClient factory configuration changes
  - Validation: Verify HttpClient injection and configuration

**5. Code Modifications**

Expected changes in the following files (based on assessment):
- Files with incidents: 6 files across the Api project
- Primary areas:
  - HTTP client configuration and usage
  - URI handling
  - WMI/System.Management queries
  - TimeSpan usage
  - SSL protocol configuration

**6. Testing Strategy**

- **Unit Tests**: Run existing unit tests for Api project (if any)
- **Integration Tests**: Test HTTP client functionality
- **Platform Tests**: Verify System.Management (WMI) queries on Windows
- **Behavioral Validation**: 
  - HTTP request/response handling
  - URI parsing and manipulation
  - JSON serialization/deserialization

**7. Validation Checklist**

- [ ] Project builds without errors
- [ ] Project builds without warnings (or warnings understood and documented)
- [ ] System.Management WMI queries execute successfully on Windows
- [ ] HTTP client operations function correctly
- [ ] URI parsing handles all existing inputs
- [ ] JSON serialization/deserialization works as expected
- [ ] Dependant projects (Maui, Simulation) build successfully

---

### SummonMyStrength.Installer

**Current State:**
- Target Framework: net472
- Project Type: WinForms (SDK-style)
- Dependencies: 0 project dependencies
- Dependants: 0
- Files: 1 file, 45 LOC
- Packages: 1 package (WixSharp_wix4 2.4.2 - compatible)
- Risk Level: Low

**Target State:**
- Target Framework: net10.0-windows
- No package updates required

#### Migration Steps

**1. Prerequisites**
- Ensure .NET 10 SDK installed with Windows workload
- Verify SummonMyStrength.Installer.csproj file is not locked

**2. Framework Update**
Change in `SummonMyStrength.Installer/SummonMyStrength.Installer.csproj`:
```xml
<TargetFramework>net10.0-windows</TargetFramework>
```
(Previously: `net472`)

**Note**: Using `net10.0-windows` because this is a Windows-specific WinForms installer project. The `-windows` suffix ensures Windows-specific APIs remain available.

**3. Package Updates**

No packages require updates:
- **WixSharp_wix4 2.4.2**: Already compatible with .NET 10

**4. Expected Breaking Changes**

Assessment shows 0 API issues. However, large framework jump from .NET Framework 4.7.2 to .NET 10 may reveal:

**Potential Issues:**
- **WinForms API changes**: .NET 10 WinForms may have API differences from .NET Framework
- **File system path handling**: .NET Core/5+ uses different default encodings
- **Platform invoke (P/Invoke)**: Any native interop may need validation

**5. Code Modifications**

Minimal expected changes:
- Project has only 45 LOC and 1 file
- Assessment found 0 API compatibility issues
- Likely just framework update required

**6. Testing Strategy**

- **Build Validation**: Verify project builds without errors
- **Installer Build**: Confirm WixSharp can generate installer package
- **Installer Execution**: Run generated installer on Windows to verify functionality
- **Uninstall Test**: Verify uninstaller works correctly

**7. Validation Checklist**

- [ ] Project builds without errors
- [ ] Project builds without warnings
- [ ] WixSharp installer package generates successfully
- [ ] Installer runs on Windows 10/11
- [ ] Application installs correctly
- [ ] Application can be uninstalled cleanly

---

### SummonMyStrength.Maui

**Current State:**
- Target Framework: net8.0-android;net8.0-ios;net8.0-maccatalyst;net8.0-windows10.0.19041.0
- Project Type: .NET MAUI App (SDK-style)
- Dependencies: 1 (SummonMyStrength.Api)
- Dependants: 0
- Files: 135 files, 2072 LOC
- Packages: 5 packages (1 requiring update)
- Risk Level: Low

**Target State:**
- Target Framework: net8.0-android;net8.0-ios;net8.0-maccatalyst;net8.0-windows10.0.19041.0;net10.0-windows (multi-targeting)
- Updated Packages: 1 package to version 10.0.5

#### Migration Steps

**1. Prerequisites**
- Ensure .NET 10 SDK installed with MAUI workload
- Ensure SummonMyStrength.Api upgraded first (dependency)
- Verify SummonMyStrength.Maui.csproj file is not locked

**2. Framework Update**
Change in `SummonMyStrength.Maui/SummonMyStrength.Maui.csproj`:
```xml
<TargetFrameworks>net8.0-android;net8.0-ios;net8.0-maccatalyst;net8.0-windows10.0.19041.0;net10.0-windows</TargetFrameworks>
```
(Append `net10.0-windows` to existing targets - multi-targeting approach)

**Note**: Assessment proposes multi-targeting to add net10.0-windows while preserving existing platform targets. This enables running on .NET 10 for Windows while maintaining .NET 8 for mobile platforms.

**Alternative**: Replace all targets with net10.0 equivalents if MAUI .NET 10 support is available for all platforms:
```xml
<TargetFrameworks>net10.0-android;net10.0-ios;net10.0-maccatalyst;net10.0-windows10.0.19041.0</TargetFrameworks>
```
Verify .NET MAUI workload supports .NET 10 for all platforms before choosing this approach.

**3. Package Updates**

| Package | Current Version | Target Version | Reason |
|---------|----------------|----------------|--------|
| Microsoft.Extensions.Logging.Debug | 8.0.0 | 10.0.5 | Framework compatibility |

**Compatible Packages (no update needed):**
- Microsoft.AspNetCore.Components.WebView.Maui - Already compatible
- Microsoft.Maui.Controls - Already compatible
- Microsoft.Maui.Controls.Compatibility - Already compatible
- MudBlazor 6.12.0 - Already compatible

**4. Expected Breaking Changes**

Assessment shows 2 source incompatible APIs:

**Source Incompatible:**

- **System.TimeSpan.FromSeconds(double)** (2 instances)
  - Issue: Overload resolution changes in .NET 9+
  - Fix: Explicitly cast to `double` if ambiguous: `TimeSpan.FromSeconds((double)value)`
  - Likely in timer or delay logic in MAUI app

**5. Code Modifications**

Expected changes:
- 2 files with incidents
- Estimated 2+ LOC to modify
- Primary areas: TimeSpan usage in UI timers or animation delays

**6. Testing Strategy**

- **Build Validation**: Build for each target framework
- **Platform Testing**: Test on:
  - Windows (net10.0-windows target)
  - Android (net8.0-android target)
  - iOS (net8.0-ios target)
  - macOS Catalyst (net8.0-maccatalyst target)
- **UI Testing**: Verify UI renders correctly on all platforms
- **Blazor Hybrid**: Test WebView and Blazor components (MudBlazor)
- **Integration**: Verify SummonMyStrength.Api integration works

**7. Validation Checklist**

- [ ] Project builds without errors for all target frameworks
- [ ] Project builds without warnings
- [ ] Application launches on Windows (.NET 10)
- [ ] Application launches on Android (.NET 8)
- [ ] Application launches on iOS (.NET 8)
- [ ] Application launches on macOS Catalyst (.NET 8)
- [ ] UI renders correctly on all platforms
- [ ] Blazor components (MudBlazor) function correctly
- [ ] Api dependency resolves correctly
- [ ] No runtime exceptions during basic workflows

---

### SummonMyStrength.Simulation

**Current State:**
- Target Framework: net8.0
- Project Type: Class Library (SDK-style)
- Dependencies: 1 (SummonMyStrength.Api)
- Dependants: 0
- Files: 18 files, 272 LOC
- Packages: 0 packages
- Risk Level: Low

**Target State:**
- Target Framework: net10.0
- No package updates required

#### Migration Steps

**1. Prerequisites**
- Ensure .NET 10 SDK installed
- Ensure SummonMyStrength.Api upgraded first (dependency)
- Verify SummonMyStrength.Simulation.csproj file is not locked

**2. Framework Update**
Change in `SummonMyStrength.Simulation/SummonMyStrength.Simulation.csproj`:
```xml
<TargetFramework>net10.0</TargetFramework>
```
(Previously: `net8.0`)

**3. Package Updates**

No packages to update (project has 0 NuGet dependencies beyond framework and Api reference).

**4. Expected Breaking Changes**

Assessment shows 0 API issues. This is the simplest upgrade in the solution.

**Potential Issues:**
- If project uses APIs from SummonMyStrength.Api that changed during Api's upgrade, compilation errors may surface
- Assessment found 1 file with incidents, but 0 estimated LOC to modify

**5. Code Modifications**

Minimal expected changes:
- 0 identified API issues
- Clean upgrade expected
- May need adjustment if Api public API changed

**6. Testing Strategy**

- **Build Validation**: Verify project builds without errors
- **Unit Tests**: Run existing unit tests for Simulation project (if any)
- **Integration Tests**: Test interaction with upgraded Api project

**7. Validation Checklist**

- [ ] Project builds without errors
- [ ] Project builds without warnings
- [ ] References to SummonMyStrength.Api resolve correctly
- [ ] Simulation logic executes correctly
- [ ] No runtime exceptions during simulation execution

---

## Package Update Reference

### Overview

4 packages require updates across 2 projects (SummonMyStrength.Api, SummonMyStrength.Maui). All other packages are already compatible with .NET 10.

### Packages Requiring Updates

| Package | Current | Target | Projects Affected | Update Reason | Priority |
|---------|---------|--------|-------------------|---------------|----------|
| Microsoft.Extensions.DependencyInjection.Abstractions | 8.0.0 | 10.0.5 | Api | Framework compatibility, bug fixes | Standard |
| Microsoft.Extensions.Http | 8.0.0 | 10.0.5 | Api | Framework compatibility, bug fixes | Standard |
| Microsoft.Extensions.Logging.Debug | 8.0.0 | 10.0.5 | Maui | Framework compatibility | Standard |
| System.Management | 5.0.0 | 10.0.5 | Api | Framework compatibility, WMI improvements | Standard |

### Compatible Packages (No Update)

| Package | Version | Projects | Reason |
|---------|---------|----------|--------|
| Microsoft.AspNetCore.Components.WebView.Maui | (implicit) | Maui | Already compatible with .NET 10 |
| Microsoft.Maui.Controls | (implicit) | Maui | Already compatible with .NET 10 |
| Microsoft.Maui.Controls.Compatibility | (implicit) | Maui | Already compatible with .NET 10 |
| MudBlazor | 6.12.0 | Maui | Already compatible with .NET 10 |
| WixSharp_wix4 | 2.4.2 | Installer | Already compatible with .NET 10 |

### Update Instructions by Project

#### SummonMyStrength.Api

Update PackageReference elements in `SummonMyStrength.Api/SummonMyStrength.Api.csproj`:

```xml
<PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="10.0.5" />
<PackageReference Include="Microsoft.Extensions.Http" Version="10.0.5" />
<PackageReference Include="System.Management" Version="10.0.5" />
```

#### SummonMyStrength.Maui

Update PackageReference elements in `SummonMyStrength.Maui/SummonMyStrength.Maui.csproj`:

```xml
<PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="10.0.5" />
```

#### SummonMyStrength.Installer

No package updates required.

#### SummonMyStrength.Simulation

No package updates required (no NuGet packages).

### Package Update Validation

After updating packages:
1. Run `dotnet restore` for entire solution
2. Verify no package dependency conflicts
3. Check for transitive dependency updates
4. Confirm all packages restore successfully

---

## Breaking Changes Catalog

### Overview

The upgrade from .NET 8 to .NET 10 (and .NET Framework 4.7.2 to .NET 10 for Installer) introduces breaking changes categorized as:
- **0 Binary Incompatible**: No changes requiring recompilation of dependent assemblies
- **10 Source Incompatible**: Changes requiring code modifications to compile
- **18 Behavioral Changes**: Changes in runtime behavior requiring testing

### Source Incompatible Changes (Require Code Fixes)

#### 1. TimeSpan.FromSeconds Overload Ambiguity

**API**: `System.TimeSpan.FromSeconds(double)`  
**Occurrences**: 2 instances (Api: 0, Maui: 2)  
**Issue**: .NET 9+ added new TimeSpan creation methods causing overload resolution ambiguity in some contexts  
**Impact**: Compilation error if argument type is ambiguous

**Fix**:
```csharp
// Before (may cause ambiguity)
var timeout = TimeSpan.FromSeconds(value);

// After (explicit cast)
var timeout = TimeSpan.FromSeconds((double)value);
```

**Projects Affected**: SummonMyStrength.Maui

---

#### 2. SslProtocols.Tls Obsolete

**API**: `System.Security.Authentication.SslProtocols.Tls`  
**Occurrences**: 1 instance (Api: 1)  
**Issue**: TLS 1.0 obsolete and removed from default allowed protocols in .NET 10  
**Impact**: Compiler warning (may become error with TreatWarningsAsErrors)

**Fix**:
```csharp
// Before
SslProtocols protocols = SslProtocols.Tls;

// After
SslProtocols protocols = SslProtocols.Tls12 | SslProtocols.Tls13;
```

**Projects Affected**: SummonMyStrength.Api

---

#### 3. SslProtocols.Tls11 Obsolete

**API**: `System.Security.Authentication.SslProtocols.Tls11`  
**Occurrences**: 1 instance (Api: 1)  
**Issue**: TLS 1.1 obsolete and removed from default allowed protocols in .NET 10  
**Impact**: Compiler warning (may become error with TreatWarningsAsErrors)

**Fix**:
```csharp
// Before
SslProtocols protocols = SslProtocols.Tls11;

// After
SslProtocols protocols = SslProtocols.Tls12 | SslProtocols.Tls13;
```

**Projects Affected**: SummonMyStrength.Api

---

#### 4. System.Management APIs - Nullable Reference Types

**APIs**: 
- `System.Management.ManagementObject`
- `System.Management.ManagementObjectCollection`
- `System.Management.ManagementObjectSearcher`
- `ManagementObjectSearcher.Get()`
- `ManagementBaseObject.Item[string]`

**Occurrences**: 6 instances total (Api: 6)  
**Issue**: System.Management 10.0.5 adds nullable reference type annotations  
**Impact**: Nullable warnings if project has `<Nullable>enable</Nullable>`

**Fix**:
```csharp
// Before
ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
foreach (ManagementObject obj in searcher.Get())
{
    var value = obj["Name"];
}

// After (add null checks)
ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
using (ManagementObjectCollection results = searcher.Get())
{
    foreach (ManagementObject obj in results)
    {
        var value = obj["Name"];
        if (value != null)
        {
            // Use value
        }
    }
}
```

**Projects Affected**: SummonMyStrength.Api

---

### Behavioral Changes (Require Testing)

#### 5. HttpContent Disposal Behavior

**API**: `System.Net.Http.HttpContent`  
**Occurrences**: 6 instances (Api: 6)  
**Change**: HttpContent disposal and stream ownership behavior refined in .NET 9+  
**Impact**: Streams may be disposed earlier or differently than in .NET 8

**Testing**:
- Verify HTTP requests complete successfully
- Check for disposed stream exceptions
- Validate response content reading
- Test custom HttpContent implementations

**Projects Affected**: SummonMyStrength.Api

---

#### 6. URI Parsing Strictness

**APIs**: 
- `System.Uri` (5 instances)
- `Uri(string)` constructor (3 instances)
- `Uri.AbsoluteUri` property (1 instance)

**Occurrences**: 9 instances total (Api: 9)  
**Change**: URI parsing more strict in .NET 9+, edge cases handled differently  
**Impact**: URIs that parsed in .NET 8 may throw exceptions or parse differently

**Testing**:
- Test all URI creation with existing inputs
- Validate URI properties (AbsoluteUri, Host, Path, etc.)
- Check for UriFormatException
- Test edge cases (empty paths, special characters, international characters)

**Projects Affected**: SummonMyStrength.Api

---

#### 7. AddHttpClient Configuration

**API**: `HttpClientFactoryServiceCollectionExtensions.AddHttpClient(IServiceCollection, string)`  
**Occurrences**: 2 instances (Api: 2)  
**Change**: HttpClient factory configuration and lifetime management changes  
**Impact**: HttpClient instances may have different lifetime or configuration

**Testing**:
- Verify HttpClient injection works
- Check HttpClient lifetime and pooling
- Validate configured HttpClient instances
- Test named HttpClient retrieval

**Projects Affected**: SummonMyStrength.Api

---

#### 8. JsonSerializer.Deserialize Behavior

**API**: `System.Text.Json.JsonSerializer.Deserialize(JsonElement, Type, JsonSerializerOptions)`  
**Occurrences**: 1 instance (Api: 1)  
**Change**: Deserialization edge case handling refined  
**Impact**: Some JSON structures may deserialize differently

**Testing**:
- Test JSON deserialization with all existing data shapes
- Validate null handling
- Check for JsonException
- Test with complex types and nested objects

**Projects Affected**: SummonMyStrength.Api

---

### Breaking Changes by Project

#### SummonMyStrength.Api
- **Source Incompatible**: 8 issues (TimeSpan: 0, SslProtocols: 2, System.Management: 6)
- **Behavioral Changes**: 18 issues (HttpContent: 6, Uri: 9, AddHttpClient: 2, JsonSerializer: 1)
- **Total**: 26 API issues

#### SummonMyStrength.Maui
- **Source Incompatible**: 2 issues (TimeSpan: 2)
- **Behavioral Changes**: 0 issues
- **Total**: 2 API issues

#### SummonMyStrength.Simulation
- **Source Incompatible**: 0 issues
- **Behavioral Changes**: 0 issues
- **Total**: 0 API issues

#### SummonMyStrength.Installer
- **Source Incompatible**: 0 issues
- **Behavioral Changes**: 0 issues
- **Total**: 0 API issues

---

### Migration Path References

#### System Management (WMI)

**Affected**: SummonMyStrength.Api (6 issues, 23.1%)

**Description**: Windows Management Instrumentation APIs for system administration and monitoring via System.Management package. Windows-only functionality.

**Migration**:
1. Update System.Management package to 10.0.5
2. Add null checks for nullable reference types
3. Test WMI queries on Windows platform
4. Consider cross-platform alternatives for new code (though existing usage is acceptable for Windows-only scenarios)

**Documentation**: https://learn.microsoft.com/en-us/dotnet/api/system.management

---

## Risk Management

### High-Level Risk Assessment

**Overall Risk: Low**

The upgrade presents minimal risk due to:
- Small solution size (4 projects)
- All projects SDK-style (modern project format)
- No security vulnerabilities identified
- High API compatibility (99.5%)
- Clear package upgrade paths
- No binary breaking changes

### Risk Factor Analysis

| Risk Factor | Assessment | Mitigation |
|-------------|------------|------------|
| Solution Complexity | Low (4 projects, simple dependencies) | All-At-Once strategy appropriate |
| API Breaking Changes | Low (0 binary, 10 source incompatible) | Breaking changes catalog with specific fixes |
| Package Compatibility | Low (all packages have .NET 10 versions) | Explicit version targets in plan |
| Framework Jump | Low (.NET 8→10, .NET Framework 4.7.2→10) | Well-documented upgrade paths |
| Multi-Targeting | Low (MAUI standard pattern) | Append net10.0-windows to existing targets |
| Test Coverage | Unknown (no test projects in assessment) | Manual validation required |
| External Dependencies | Low (9 packages total, 4 updates) | All Microsoft packages with LTS support |

### Specific Risks by Project

#### SummonMyStrength.Api
- **Risk Level**: Medium-Low
- **Issues**: 26 API issues (8 source incompatible, 18 behavioral changes)
- **Key Concern**: System.Management APIs (6 instances) - Windows-specific WMI functionality
- **Mitigation**: 
  - Update System.Management package to 10.0.5
  - Verify WMI query functionality post-upgrade
  - Test on Windows platform specifically

#### SummonMyStrength.Installer
- **Risk Level**: Low
- **Issues**: 0 API issues
- **Key Concern**: Large framework jump (.NET Framework 4.7.2 → .NET 10)
- **Mitigation**:
  - TargetFramework change to net10.0-windows (Windows-specific)
  - WixSharp_wix4 package already compatible
  - Verify installer build process

#### SummonMyStrength.Maui
- **Risk Level**: Low
- **Issues**: 2 source incompatible APIs (TimeSpan.FromSeconds)
- **Key Concern**: Multi-targeting with net10.0-windows addition
- **Mitigation**:
  - Append net10.0-windows to TargetFrameworks (keep existing targets)
  - Update Microsoft.Extensions.Logging.Debug to 10.0.5
  - Test on all target platforms

#### SummonMyStrength.Simulation
- **Risk Level**: Low
- **Issues**: 0 API issues, 0 package updates
- **Key Concern**: None (simplest upgrade)
- **Mitigation**: Standard framework update, depends on Api building successfully

### Contingency Plans

**If Build Fails After Framework Update:**
1. Examine build errors for API incompatibilities not caught by assessment
2. Consult breaking changes catalog for .NET 9 and .NET 10
3. Check for conditional compilation issues (#if directives)
4. Verify package restore completed successfully

**If System.Management Issues Arise:**
- Package is Windows-only; ensure net10.0 build targets Windows
- Consider platform-specific compilation if cross-platform support needed
- Alternative: Move WMI functionality to separate Windows-only library

**If MAUI Multi-Targeting Issues Arise:**
- Verify each target framework builds independently
- Check for platform-specific code compatibility
- Consult .NET MAUI .NET 10 migration guide

**Rollback Strategy:**
- All changes on dedicated branch `upgrade-to-NET10`
- Single atomic commit enables clean revert
- Original state preserved on `master` branch

---

## Testing & Validation Strategy

### Overview

Since no automated test projects were identified in the assessment, validation relies on manual testing and compilation verification. Testing follows a multi-level approach aligned with the All-At-Once strategy.

### Level 1: Compilation Validation

**Objective**: Verify solution builds without errors

**Process**:
1. Run `dotnet restore` for entire solution
2. Run `dotnet build` for entire solution
3. Address all compilation errors
4. Address or document all warnings
5. Verify 0 errors across all projects

**Success Criteria**:
- All 4 projects build successfully
- No compilation errors
- Warnings understood and documented (or resolved)
- No package dependency conflicts

---

### Level 2: Project-Level Validation

**Objective**: Verify each project individually

#### SummonMyStrength.Api

**Build Tests**:
- [ ] Project builds with `dotnet build SummonMyStrength.Api/SummonMyStrength.Api.csproj`
- [ ] No errors or warnings

**Functional Tests**:
- [ ] System.Management WMI queries execute (e.g., processor information query)
- [ ] HTTP client creation and configuration works
- [ ] URI parsing handles expected inputs
- [ ] JSON serialization/deserialization functions correctly
- [ ] Dependency injection container resolves services

**Platform Tests**:
- [ ] Test on Windows (required for System.Management)

---

#### SummonMyStrength.Installer

**Build Tests**:
- [ ] Project builds with `dotnet build SummonMyStrength.Installer/SummonMyStrength.Installer.csproj`
- [ ] No errors or warnings
- [ ] WixSharp generates installer package

**Functional Tests**:
- [ ] Installer MSI/package builds successfully
- [ ] Installer runs on Windows 10/11
- [ ] Application installs to expected location
- [ ] Application launches after installation
- [ ] Uninstaller removes application cleanly

**Platform Tests**:
- [ ] Test on Windows 10
- [ ] Test on Windows 11

---

#### SummonMyStrength.Maui

**Build Tests**:
- [ ] Project builds for net8.0-android
- [ ] Project builds for net8.0-ios
- [ ] Project builds for net8.0-maccatalyst
- [ ] Project builds for net8.0-windows10.0.19041.0
- [ ] Project builds for net10.0-windows (new target)
- [ ] No errors or warnings

**Functional Tests**:
- [ ] Application launches on Windows (net10.0-windows)
- [ ] Application launches on Android emulator/device
- [ ] Application launches on iOS simulator/device
- [ ] Application launches on macOS Catalyst
- [ ] UI renders correctly on all platforms
- [ ] Navigation works
- [ ] Blazor components (MudBlazor) render and function
- [ ] Integration with SummonMyStrength.Api works (dependency)

**Platform Tests**:
- [ ] Windows (net10.0-windows target)
- [ ] Android (net8.0-android target)
- [ ] iOS (net8.0-ios target)
- [ ] macOS Catalyst (net8.0-maccatalyst target)

---

#### SummonMyStrength.Simulation

**Build Tests**:
- [ ] Project builds with `dotnet build SummonMyStrength.Simulation/SummonMyStrength.Simulation.csproj`
- [ ] No errors or warnings

**Functional Tests**:
- [ ] Simulation logic executes without exceptions
- [ ] Integration with SummonMyStrength.Api works (dependency)
- [ ] Simulation results are consistent with .NET 8 behavior

---

### Level 3: Solution-Level Integration

**Objective**: Verify entire solution works together

**Integration Tests**:
- [ ] Full solution builds with `dotnet build SummonMyStrength.sln`
- [ ] All projects build in correct dependency order
- [ ] MAUI app can call Api project successfully
- [ ] Simulation project can call Api project successfully
- [ ] No runtime exceptions in integrated workflows

**Smoke Tests**:
- [ ] Launch MAUI app and navigate through main screens
- [ ] Execute at least one simulation
- [ ] Verify API calls from MAUI app succeed
- [ ] Check for obvious UI or functional regressions

---

### Level 4: Behavioral Change Validation

**Objective**: Verify behavioral changes don't cause regressions

#### HttpContent Disposal (6 instances in Api)
- [ ] HTTP requests complete successfully
- [ ] Response content readable
- [ ] No "Cannot access disposed object" exceptions
- [ ] Custom HttpContent implementations (if any) work

#### URI Parsing (9 instances in Api)
- [ ] All existing URI inputs parse successfully
- [ ] URI properties (AbsoluteUri, Host, Path) have expected values
- [ ] No UriFormatException exceptions for valid URIs
- [ ] International/special character URIs handled

#### AddHttpClient Configuration (2 instances in Api)
- [ ] HttpClient injection works via DI container
- [ ] Named HttpClient instances retrievable
- [ ] HttpClient lifetime/pooling behaves correctly
- [ ] Configured settings (timeout, headers) applied

#### JsonSerializer.Deserialize (1 instance in Api)
- [ ] JSON deserialization succeeds for all data types
- [ ] Null handling correct
- [ ] Complex/nested objects deserialize correctly
- [ ] No unexpected JsonException

---

### Level 5: Performance & Quality

**Objective**: Ensure upgrade doesn't degrade performance or quality

**Performance Tests** (if applicable):
- [ ] Application startup time comparable to .NET 8
- [ ] API response times within acceptable range
- [ ] MAUI app responsiveness maintained
- [ ] Memory usage reasonable

**Quality Checks**:
- [ ] No new compiler warnings introduced
- [ ] Code analysis passes (if enabled)
- [ ] No new static analysis warnings
- [ ] Documentation updated (README, deployment docs, etc.)

---

### Test Execution Order

For All-At-Once strategy, testing proceeds in this sequence:

1. **Compilation Validation** - Must pass before proceeding
2. **Project-Level Validation** - Test each project individually
3. **Solution-Level Integration** - Test projects together
4. **Behavioral Change Validation** - Focused testing on known changes
5. **Performance & Quality** - Final validation

---

### Testing Responsibilities

**Developer**:
- Execute all compilation and build tests
- Perform project-level functional tests
- Validate behavioral changes
- Document any issues or deviations

**QA/Testing** (if available):
- Execute platform-specific tests (Android, iOS, macOS)
- Perform comprehensive MAUI app testing
- Validate installer functionality
- Execute performance tests

---

### Test Environment Requirements

**Software**:
- .NET 10 SDK
- Visual Studio 2025 (or latest with .NET 10 support)
- .NET MAUI workload installed
- WiX Toolset (for installer builds)

**Hardware/Platforms**:
- Windows 10/11 machine (for System.Management, Installer, MAUI Windows)
- Android emulator or physical device
- iOS simulator or physical device (requires macOS)
- macOS machine (for Catalyst testing)

---

### Failure Response

**If Tests Fail**:
1. Document failure details (project, test, error message)
2. Consult Breaking Changes Catalog for known issues
3. Review .NET 10 migration documentation
4. Fix issues and re-run affected tests
5. If blocking issue, escalate or consider rollback

**Rollback Criteria**:
- Critical functionality broken and no fix available
- Performance degradation >30%
- Platform-specific failures with no workaround
- Timeline constraints preventing timely resolution

---

## Complexity & Effort Assessment

### Relative Complexity Ratings

The All-At-Once strategy treats all projects as a single coordinated unit, but individual project complexity varies:

| Project | Complexity | Dependencies | Risk Factors | Justification |
|---------|-----------|--------------|--------------|---------------|
| **SummonMyStrength.Api** | Medium | 0 | 26 API issues, 4 package updates, System.Management WMI APIs | Highest code change impact |
| **SummonMyStrength.Installer** | Low | 0 | Large framework jump but minimal code | Small codebase (45 LOC) |
| **SummonMyStrength.Maui** | Medium | 1 | Multi-targeting, 2 API issues, 1 package update | MAUI complexity, platform testing |
| **SummonMyStrength.Simulation** | Low | 1 | 0 issues, 0 package updates | Straightforward framework change |

### Phase Complexity Assessment

**Phase 0: Preparation**
- **Complexity**: Low
- **Operations**: Verify .NET 10 SDK, confirm branch
- **Dependencies**: None
- **Estimated Scope**: Environmental checks

**Phase 1: Atomic Upgrade**
- **Complexity**: Medium
- **Operations**: Update 4 project files, update 4 packages, fix 10 source incompatibilities, validate 18 behavioral changes
- **Dependencies**: All projects updated together; dependency order respected by build system
- **Estimated Scope**: 
  - Project file edits: 4 files
  - Package updates: 4 packages across 2 projects
  - Code fixes: ~28 LOC across 6 files
  - Build + error resolution

**Phase 2: Validation**
- **Complexity**: Low-Medium
- **Operations**: Solution build validation, manual testing (no automated tests identified)
- **Dependencies**: Phase 1 complete
- **Estimated Scope**: Full solution build, MAUI app launch validation, functional testing

### Resource Requirements

**Skills Required:**
- .NET upgrade experience (understanding of framework changes)
- C# development (for compilation error fixes)
- .NET MAUI knowledge (for multi-targeting validation)
- Windows development (for System.Management and Installer testing)

**Parallel Capacity:**
- All-At-Once strategy is inherently sequential within the atomic upgrade phase
- Single developer can complete entire upgrade
- Testing may benefit from multiple platforms (Windows, Android, iOS, macOS)

### Complexity Drivers

**Primary Complexity Factors:**
1. **API Behavioral Changes** (18 instances) - Requires runtime validation
2. **System.Management APIs** (6 instances) - Windows-specific, needs platform testing
3. **MAUI Multi-Targeting** - Adds net10.0-windows to existing 4 targets
4. **Large Framework Jump** (Installer: .NET Framework 4.7.2 → .NET 10) - Potential for unexpected issues

**Simplifying Factors:**
1. All projects SDK-style (no project file conversion needed)
2. No binary breaking changes (only source incompatibilities)
3. All packages have clear .NET 10 versions
4. Small codebase (5,245 LOC total)
5. Simple dependency graph (no cycles, max depth 2)

---

## Source Control Strategy

### Branching Strategy

**Main Branch**: `master`  
**Source Branch**: `master`  
**Upgrade Branch**: `upgrade-to-NET10` (already created)

**Branch Purpose**:
- `master`: Stable .NET 8 / .NET Framework 4.7.2 codebase
- `upgrade-to-NET10`: .NET 10 upgrade work (isolated from main development)

**Merge Approach**:
- Complete upgrade on `upgrade-to-NET10` branch
- All testing and validation on upgrade branch
- Merge to `master` only after full validation passes
- Use Pull Request for final review before merge

---

### Commit Strategy

**Recommended: Single Atomic Commit**

Since this is an All-At-Once upgrade, prefer one comprehensive commit containing all changes:

**Commit Structure**:
```
Upgrade solution to .NET 10

- Update all project files to .NET 10 target frameworks
  - Api: net8.0 → net10.0
  - Installer: net472 → net10.0-windows
  - Maui: add net10.0-windows to multi-targeting
  - Simulation: net8.0 → net10.0

- Update NuGet packages to .NET 10 compatible versions
  - Microsoft.Extensions.DependencyInjection.Abstractions: 8.0.0 → 10.0.5
  - Microsoft.Extensions.Http: 8.0.0 → 10.0.5
  - Microsoft.Extensions.Logging.Debug: 8.0.0 → 10.0.5
  - System.Management: 5.0.0 → 10.0.5

- Fix source incompatibilities
  - Update TimeSpan.FromSeconds calls (explicit double cast)
  - Replace obsolete SslProtocols.Tls/Tls11 with Tls12/Tls13
  - Add null checks for System.Management APIs

- Validate all projects build successfully
- All behavioral changes tested and verified
```

**Advantages**:
- Easy rollback (single commit to revert)
- Clear atomic change boundary
- Simplified code review
- Clean history

**Alternative: Phased Commits** (if atomic commit too large):
1. Commit: Update project files (framework versions only)
2. Commit: Update NuGet packages
3. Commit: Fix compilation errors and source incompatibilities
4. Commit: Address any additional issues found during testing

---

### Commit Message Format

**Format**:
```
<type>: <subject>

<body>

<footer>
```

**Example**:
```
upgrade: Migrate solution to .NET 10

All projects upgraded from .NET 8/.NET Framework 4.7.2 to .NET 10.
Package references updated to compatible versions.
Source incompatibilities addressed (TimeSpan, SslProtocols, System.Management).

Validated:
- All projects build with 0 errors
- MAUI app launches on all platforms
- Installer builds successfully
- API functionality tested

Refs: #<issue-number-if-any>
```

---

### Review and Merge Process

#### Pre-Merge Checklist

Before creating Pull Request from `upgrade-to-NET10` to `master`:

**Code Quality**:
- [ ] All projects build without errors
- [ ] All projects build without warnings (or warnings documented)
- [ ] Code formatting consistent with project standards
- [ ] No commented-out code or debug statements

**Testing**:
- [ ] All validation tests passed (see Testing & Validation Strategy)
- [ ] Behavioral changes verified
- [ ] Platform-specific tests completed
- [ ] No known regressions

**Documentation**:
- [ ] README updated with .NET 10 requirements
- [ ] Build instructions updated
- [ ] Deployment documentation updated (if SDK version matters)
- [ ] CHANGELOG or release notes updated

**Source Control**:
- [ ] All changes committed
- [ ] Commit messages clear and descriptive
- [ ] No merge conflicts with master
- [ ] Branch up-to-date with latest master

#### Pull Request Template

**Title**: `Upgrade solution to .NET 10`

**Description**:
```markdown
## Summary
Upgrades all projects in SummonMyStrength solution to .NET 10.

## Changes
- **SummonMyStrength.Api**: net8.0 → net10.0 + 4 package updates
- **SummonMyStrength.Installer**: net472 → net10.0-windows
- **SummonMyStrength.Maui**: Added net10.0-windows to multi-targeting + 1 package update
- **SummonMyStrength.Simulation**: net8.0 → net10.0

## Breaking Changes Addressed
- TimeSpan.FromSeconds overload ambiguity (2 instances)
- Obsolete SslProtocols.Tls/Tls11 (2 instances)
- System.Management nullable annotations (6 instances)

## Testing Completed
- [x] All projects build successfully
- [x] MAUI app tested on Windows/Android/iOS/macOS
- [x] Installer builds and runs
- [x] API functionality validated
- [x] Behavioral changes tested

## Validation Criteria Met
- ✅ Solution builds with 0 errors
- ✅ All packages updated to .NET 10 compatible versions
- ✅ All source incompatibilities resolved
- ✅ Runtime testing completed

## Deployment Notes
- Requires .NET 10 SDK for builds
- MAUI workload required for MAUI project
- WiX Toolset for installer builds

## Rollback Plan
Single atomic commit enables clean revert if issues discovered post-merge.
```

#### Review Criteria

**Reviewers should verify**:
- All framework versions updated correctly
- Package versions appropriate for .NET 10
- Breaking change fixes are correct
- No unintended code changes
- Tests executed and documented
- Build/deployment documentation updated

**Approval Requirements**:
- At least 1 code review approval
- All CI/CD checks passing (if configured)
- QA sign-off (if QA team exists)

---

### Merge Execution

**Merge Method**: Squash and Merge (recommended)
- Combines all commits into single commit on master
- Clean history
- Easy identification of upgrade change

**Alternative**: Merge Commit
- Preserves individual commits if phased approach used
- Creates merge commit on master

**Post-Merge**:
1. Verify master builds successfully
2. Tag release: `git tag v<version>-net10`
3. Update deployment pipelines to use .NET 10 SDK
4. Notify team of .NET 10 upgrade completion
5. Delete `upgrade-to-NET10` branch (optional, or keep for reference)

---

### Rollback Procedure

**If Issues Discovered Post-Merge**:

**Immediate Rollback**:
```bash
git revert <merge-commit-hash>
git push origin master
```

**Alternative: Revert to Pre-Merge State**:
```bash
git reset --hard <commit-before-merge>
git push --force origin master  # Use with caution
```

**After Rollback**:
1. Recreate `upgrade-to-NET10` branch from master
2. Fix identified issues
3. Re-test thoroughly
4. Attempt merge again

---

### Branch Protection

**Recommended Settings for `master`**:
- Require pull request reviews
- Require status checks to pass (if CI/CD configured)
- Require branches to be up to date before merging
- No force pushes
- No deletions

---

## Success Criteria

The .NET 10 upgrade is considered successful when all criteria below are met.

---

### Technical Criteria

#### Framework Migration

- ✅ **SummonMyStrength.Api** targets `net10.0`
- ✅ **SummonMyStrength.Installer** targets `net10.0-windows`
- ✅ **SummonMyStrength.Maui** targets `net8.0-android;net8.0-ios;net8.0-maccatalyst;net8.0-windows10.0.19041.0;net10.0-windows` (or all net10.0 if MAUI supports)
- ✅ **SummonMyStrength.Simulation** targets `net10.0`

#### Package Updates

- ✅ **Microsoft.Extensions.DependencyInjection.Abstractions** updated to `10.0.5` in Api
- ✅ **Microsoft.Extensions.Http** updated to `10.0.5` in Api
- ✅ **Microsoft.Extensions.Logging.Debug** updated to `10.0.5` in Maui
- ✅ **System.Management** updated to `10.0.5` in Api
- ✅ All compatible packages remain at current versions (WixSharp_wix4, MudBlazor, MAUI packages)

#### Compilation Success

- ✅ `dotnet restore` completes without errors for entire solution
- ✅ `dotnet build` completes with **0 errors** for entire solution
- ✅ No package dependency conflicts
- ✅ No warnings related to upgrade (or warnings documented and acceptable)

#### Breaking Changes Addressed

**Source Incompatible (10 instances)**:
- ✅ TimeSpan.FromSeconds ambiguity resolved (2 instances in Maui)
- ✅ SslProtocols.Tls obsolete usage replaced (1 instance in Api)
- ✅ SslProtocols.Tls11 obsolete usage replaced (1 instance in Api)
- ✅ System.Management nullable annotations handled (6 instances in Api)

**Behavioral Changes (18 instances)**:
- ✅ HttpContent disposal tested (6 instances in Api)
- ✅ URI parsing tested (9 instances in Api)
- ✅ AddHttpClient configuration tested (2 instances in Api)
- ✅ JsonSerializer.Deserialize tested (1 instance in Api)

---

### Quality Criteria

#### Code Quality

- ✅ No regression in code quality or readability
- ✅ Code formatting consistent with project standards
- ✅ No dead code or debug statements introduced
- ✅ Static analysis passes (if enabled)

#### Test Coverage

Since no automated test projects exist:
- ✅ Manual testing completed for all projects
- ✅ All validation checklists completed (per project)
- ✅ Platform-specific testing completed (Windows, Android, iOS, macOS)

#### Performance

- ✅ Application startup time acceptable
- ✅ Runtime performance within expected ranges
- ✅ No obvious performance regressions
- ✅ Memory usage reasonable

#### Documentation

- ✅ README updated with .NET 10 requirements
- ✅ Build instructions reflect .NET 10 SDK requirement
- ✅ Deployment documentation updated
- ✅ CHANGELOG or release notes document upgrade
- ✅ This plan.md completed and accurate

---

### Process Criteria

#### Source Control

- ✅ All changes on `upgrade-to-NET10` branch
- ✅ Commits follow project conventions
- ✅ Commit messages clear and descriptive
- ✅ Pull Request created with complete description
- ✅ Code review completed and approved

#### All-At-Once Strategy Compliance

- ✅ All projects upgraded simultaneously in single atomic operation
- ✅ No intermediate multi-targeting states (except MAUI's standard pattern)
- ✅ Single coordinated commit (or clearly phased commits if needed)
- ✅ Solution testable as complete unit

#### Validation

- ✅ **SummonMyStrength.Api**: Builds, WMI queries work, HTTP clients function, URIs parse correctly, JSON serialization works
- ✅ **SummonMyStrength.Installer**: Builds, installer package generates, installer runs and installs successfully
- ✅ **SummonMyStrength.Maui**: Builds for all targets, launches on all platforms, UI renders correctly, Blazor components work
- ✅ **SummonMyStrength.Simulation**: Builds, executes without errors, integrates with Api

---

### Functional Criteria

#### SummonMyStrength.Api

- ✅ All APIs function as expected
- ✅ HTTP client operations succeed
- ✅ URI parsing handles all inputs
- ✅ JSON serialization/deserialization works
- ✅ System.Management WMI queries execute on Windows
- ✅ Dependency injection resolves services correctly

#### SummonMyStrength.Installer

- ✅ Installer builds successfully
- ✅ Installer runs on Windows 10/11
- ✅ Application installs correctly
- ✅ Application launches post-install
- ✅ Uninstaller works cleanly

#### SummonMyStrength.Maui

- ✅ Application launches on Windows (net10.0-windows)
- ✅ Application launches on Android
- ✅ Application launches on iOS
- ✅ Application launches on macOS Catalyst
- ✅ UI renders on all platforms
- ✅ Navigation works
- ✅ Blazor components (MudBlazor) function correctly
- ✅ Integration with Api project works

#### SummonMyStrength.Simulation

- ✅ Simulation logic executes
- ✅ Integration with Api project works
- ✅ Results consistent with expectations

---

### Deployment Criteria

- ✅ Build pipelines updated to use .NET 10 SDK
- ✅ Deployment documentation reflects .NET 10 requirements
- ✅ Team notified of .NET 10 upgrade
- ✅ Development environments updated (.NET 10 SDK installed)

---

### Risk Mitigation Criteria

- ✅ No security vulnerabilities introduced
- ✅ No new compiler warnings related to security
- ✅ All obsolete API usages addressed (SslProtocols.Tls, Tls11)
- ✅ Rollback plan documented and tested (if possible)

---

### Sign-Off

**Developer Sign-Off**:
- [ ] All technical criteria met
- [ ] All quality criteria met
- [ ] All functional criteria met

**QA Sign-Off** (if applicable):
- [ ] All testing completed
- [ ] No blocking issues
- [ ] Acceptable for production

**Project Lead Sign-Off**:
- [ ] All criteria reviewed
- [ ] Upgrade approved for merge to master
- [ ] Deployment timeline confirmed

---

### Definition of Done

The upgrade is **DONE** when:

1. All checkboxes above are ✅ (checked)
2. Pull Request approved and merged to `master`
3. `master` branch builds successfully with .NET 10
4. Team notified and development environments updated
5. No critical issues reported within initial deployment period

**Final Validation**: After merge, verify production deployment (or equivalent) works with .NET 10 runtime.
