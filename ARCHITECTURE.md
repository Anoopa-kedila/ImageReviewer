# ImageReviewer - Architecture Documentation

## Table of Contents

1. [High-Level Design (HLD)](#high-level-design-hld)
2. [Low-Level Design (LLD)](#low-level-design-lld)
3. [Class Diagrams](#class-diagrams)
4. [Sequence Diagrams](#sequence-diagrams)
5. [Component Architecture](#component-architecture)
6. [Data Flow Diagrams](#data-flow-diagrams)
7. [Deployment Architecture](#deployment-architecture)

---

## High-Level Design (HLD)

### System Architecture Overview

```mermaid
graph TB
    subgraph "Presentation Layer"
        UI[WPF UI - XAML Views]
        VM[ViewModels]
        CONV[Converters]
        CMD[Commands]
    end
    
    subgraph "Business Logic Layer"
        SVC[Services]
        FAC[Factories]
        OPS[Operations]
    end
    
    subgraph "Domain Layer"
        MOD[Models]
        CONT[Contracts/Interfaces]
        ENUM[Enums]
    end
    
    subgraph "Infrastructure Layer"
        IO[File I/O]
        DIAL[Dialogs]
        UTIL[Utilities]
    end
    
    UI --> VM
    VM --> CMD
    VM --> SVC
    SVC --> FAC
    FAC --> OPS
    OPS --> MOD
    SVC --> IO
    SVC --> DIAL
    VM --> CONV
    
    style UI fill:#e1f5ff
    style VM fill:#b3e5fc
    style SVC fill:#fff9c4
    style OPS fill:#ffecb3
    style MOD fill:#f0f4c3
```

### Technology Stack

```mermaid
graph LR
    subgraph "Frontend"
        WPF[WPF Framework]
        XAML[XAML Markup]
    end
    
    subgraph "Backend"
        NET[.NET 10]
        CSHARP[C# 12]
    end
    
    subgraph "Patterns"
        MVVM[MVVM Pattern]
        DI[Dependency Injection]
        STRAT[Strategy Pattern]
    end
    
    subgraph "Testing"
        XUNIT[xUnit]
        MOQ[Moq]
        FA[FluentAssertions]
    end
    
    WPF --> NET
    XAML --> WPF
    NET --> CSHARP
    MVVM --> WPF
    DI --> NET
    STRAT --> OPS[Image Operations]
    XUNIT --> TEST[Unit Tests]
    MOQ --> TEST
    FA --> TEST
```

---

## Low-Level Design (LLD)

### MVVM Architecture Details

```mermaid
graph TB
    subgraph "View Layer"
        MW[MainWindow.xaml]
        APP[App.xaml]
        STYLES[Style Resources]
    end
    
    subgraph "ViewModel Layer"
        MVM[MainViewModel]
        OPM[OperationParameterManager]
        WVM[WindowLevelParametersViewModel]
        GVM[GammaParametersViewModel]
        GFVM[GaussianFilterParametersViewModel]
        MFVM[MedianFilterParametersViewModel]
        TVM[ThresholdParametersViewModel]
        BPVM[BadPixelSuppressionParametersViewModel]
    end
    
    subgraph "Model Layer"
        ID[ImageData]
        IM[ImageMetadata]
        IS[ImageStatistics]
        WLP[WindowLevelParameters]
        GP[GammaParameters]
        GFP[GaussianFilterParameters]
    end
    
    MW --> MVM
    MVM --> OPM
    OPM --> WVM
    OPM --> GVM
    OPM --> GFVM
    MVM --> ID
    MVM --> IM
    ID --> IS
    WVM --> WLP
    GVM --> GP
    GFVM --> GFP
    
    style MW fill:#e3f2fd
    style MVM fill:#bbdefb
    style ID fill:#c8e6c9
```

### Service Architecture

```mermaid
graph LR
    subgraph "Service Interfaces"
        IIL[IImageLoaderService]
        IIS[IImageSaveService]
        IIO[IImageOperation]
        IIF[IImageOperationFactory]
        IDS[IDialogService]
    end
    
    subgraph "Service Implementations"
        ILS[ImageLoaderService]
        ISS[ImageSaveService]
        WLO[WindowLevelOperation]
        GCO[GammaCorrectionOperation]
        GFO[GaussianFilterOperation]
        MFO[MedianFilterOperation]
        TO[ThresholdingOperation]
        BPO[BadPixelSuppressionOperation]
        IOF[ImageOperationFactory]
        DS[DialogService]
    end
    
    IIL -.implements.-> ILS
    IIS -.implements.-> ISS
    IIO -.implements.-> WLO
    IIO -.implements.-> GCO
    IIO -.implements.-> GFO
    IIO -.implements.-> MFO
    IIO -.implements.-> TO
    IIO -.implements.-> BPO
    IIF -.implements.-> IOF
    IDS -.implements.-> DS
    
    style IIL fill:#fff3e0
    style ILS fill:#ffe0b2
```

---

## Class Diagrams

### Core Domain Model

```mermaid
classDiagram
    class ImageData {
        +Memory~ushort~ PixelData
        +int Width
        +int Height
        +int BitDepth
        +ImageData(ushort[] pixels, int w, int h, int depth)
        +Clone() ImageData
        +GetPixel(int x, int y) ushort
        +SetPixel(int x, int y, ushort value)
    }
    
    class ImageMetadata {
        +string FileName
        +int Width
        +int Height
        +int BitDepth
        +ushort MinIntensity
        +ushort MaxIntensity
        +double MeanIntensity
        +ImageStatistics Statistics
    }
    
    class ImageStatistics {
        +ushort Min
        +ushort Max
        +double Mean
        +double StandardDeviation
        +long TotalPixels
        +Calculate(ImageData image) ImageStatistics
    }
    
    ImageMetadata --> ImageStatistics
    ImageMetadata ..> ImageData : uses
```

### ViewModel Hierarchy

```mermaid
classDiagram
    class INotifyPropertyChanged {
        <<interface>>
        +PropertyChanged Event
    }
    
    class MainViewModel {
        -IImageLoaderService _imageLoader
        -IImageSaveService _imageSaver
        -IImageOperationFactory _operationFactory
        -IDialogService _dialogService
        -ImageData _originalImage
        -ImageData _processedImage
        +BitmapSource OriginalDisplayImage
        +BitmapSource ProcessedDisplayImage
        +ImageMetadata ImageMetadata
        +List~string~ AvailableOperations
        +string SelectedOperation
        +bool IsProcessing
        +AsyncRelayCommand LoadImageCommand
        +AsyncRelayCommand ApplyOperationCommand
        +RelayCommand ResetCommand
        +AsyncRelayCommand SaveImageCommand
        -LoadImageAsync() Task
        -ApplyOperationAsync() Task
        -Reset() void
        -SaveImageAsync() Task
    }
    
    class IOperationParametersViewModel {
        <<interface>>
        +Validate() bool
        +GetParameters() object
    }
    
    class WindowLevelParametersViewModel {
        +double Window
        +double Level
        +double MinWindow
        +double MaxWindow
        +Validate() bool
        +GetParameters() WindowLevelParameters
    }
    
    class GammaParametersViewModel {
        +double Gamma
        +double MinGamma
        +double MaxGamma
        +Validate() bool
        +GetParameters() GammaParameters
    }
    
    INotifyPropertyChanged <|.. MainViewModel
    INotifyPropertyChanged <|.. IOperationParametersViewModel
    IOperationParametersViewModel <|.. WindowLevelParametersViewModel
    IOperationParametersViewModel <|.. GammaParametersViewModel
    MainViewModel --> IOperationParametersViewModel : uses
```

### Operation Strategy Pattern

```mermaid
classDiagram
    class IImageOperation {
        <<interface>>
        +string Name
        +Execute(ImageData image, IProgress progress, CancellationToken ct) ImageData
    }
    
    class WindowLevelOperation {
        -double _window
        -double _level
        +string Name
        +Execute(ImageData, IProgress, CancellationToken) ImageData
        -BuildLookupTable() ushort[]
    }
    
    class GammaCorrectionOperation {
        -double _gamma
        +string Name
        +Execute(ImageData, IProgress, CancellationToken) ImageData
        -BuildLookupTable() ushort[]
    }
    
    class GaussianFilterOperation {
        -int _kernelSize
        -double _sigma
        +string Name
        +Execute(ImageData, IProgress, CancellationToken) ImageData
        -CreateKernel() double[]
        -ApplySeparableConvolution() ImageData
    }
    
    class MedianFilterOperation {
        -int _kernelSize
        +string Name
        +Execute(ImageData, IProgress, CancellationToken) ImageData
        -GetMedian(ushort[] values) ushort
    }
    
    class ImageOperationFactory {
        +CreateOperation(ImageOperation type, object parameters) IImageOperation
        +GetAvailableOperations() List~ImageOperation~
    }
    
    IImageOperation <|.. WindowLevelOperation
    IImageOperation <|.. GammaCorrectionOperation
    IImageOperation <|.. GaussianFilterOperation
    IImageOperation <|.. MedianFilterOperation
    ImageOperationFactory ..> IImageOperation : creates
```

### Command Pattern Implementation

```mermaid
classDiagram
    class ICommand {
        <<interface>>
        +CanExecute(object parameter) bool
        +Execute(object parameter) void
        +CanExecuteChanged Event
    }
    
    class RelayCommand {
        -Action~object~ _execute
        -Func~object,bool~ _canExecute
        +RelayCommand(Action execute, Func canExecute)
        +CanExecute(object parameter) bool
        +Execute(object parameter) void
        +RaiseCanExecuteChanged() void
    }
    
    class AsyncRelayCommand {
        -Func~Task~ _execute
        -Func~bool~ _canExecute
        -bool _isExecuting
        +AsyncRelayCommand(Func execute, Func canExecute)
        +CanExecute(object parameter) bool
        +Execute(object parameter) void
        +ExecuteAsync() Task
        +RaiseCanExecuteChanged() void
    }
    
    ICommand <|.. RelayCommand
    ICommand <|.. AsyncRelayCommand
```

---

## Sequence Diagrams

### Load Image Workflow

```mermaid
sequenceDiagram
    participant User
    participant MainWindow
    participant MainViewModel
    participant DialogService
    participant ImageLoaderService
    participant ImageData
    
    User->>MainWindow: Click "Load Image"
    MainWindow->>MainViewModel: LoadImageCommand.Execute()
    MainViewModel->>DialogService: ShowOpenFileDialog()
    DialogService-->>MainViewModel: filePath
    
    alt File Selected
        MainViewModel->>ImageLoaderService: LoadImageAsync(filePath)
        ImageLoaderService->>ImageData: new ImageData(pixels, w, h)
        ImageData-->>ImageLoaderService: imageData
        ImageLoaderService-->>MainViewModel: imageData
        MainViewModel->>MainViewModel: CalculateStatistics()
        MainViewModel->>MainViewModel: ConvertToDisplayImage()
        MainViewModel-->>MainWindow: Update UI (PropertyChanged)
        MainWindow-->>User: Display Image
    else No File
        DialogService-->>MainViewModel: null
        MainViewModel-->>User: No action
    end
```

### Apply Operation Workflow

```mermaid
sequenceDiagram
    participant User
    participant MainWindow
    participant MainViewModel
    participant OperationParameterManager
    participant ImageOperationFactory
    participant IImageOperation
    participant ImageData
    
    User->>MainWindow: Select Operation
    MainWindow->>MainViewModel: SelectedOperation = "Gaussian Filter"
    MainViewModel->>OperationParameterManager: UpdateParameters(operation)
    OperationParameterManager-->>MainWindow: Display Parameter UI
    
    User->>MainWindow: Adjust Parameters
    MainWindow->>OperationParameterManager: Update values
    
    User->>MainWindow: Click "Apply"
    MainWindow->>MainViewModel: ApplyOperationCommand.Execute()
    MainViewModel->>OperationParameterManager: GetParameters()
    OperationParameterManager-->>MainViewModel: parameters
    
    MainViewModel->>ImageOperationFactory: CreateOperation(type, params)
    ImageOperationFactory-->>MainViewModel: operation
    
    MainViewModel->>MainViewModel: Set IsProcessing = true
    MainViewModel->>IImageOperation: Execute(originalImage, progress, ct)
    
    loop Processing
        IImageOperation->>MainViewModel: ReportProgress(percentage)
        MainViewModel-->>MainWindow: Update ProgressBar
    end
    
    IImageOperation-->>MainViewModel: processedImage
    MainViewModel->>MainViewModel: ConvertToDisplayImage()
    MainViewModel->>MainViewModel: Set IsProcessing = false
    MainViewModel-->>MainWindow: Update UI
    MainWindow-->>User: Display Processed Image
```

### Save Image Workflow

```mermaid
sequenceDiagram
    participant User
    participant MainWindow
    participant MainViewModel
    participant DialogService
    participant ImageSaveService
    participant FileSystem
    
    User->>MainWindow: Click "Save Image"
    MainWindow->>MainViewModel: SaveImageCommand.Execute()
    MainViewModel->>DialogService: ShowSaveFileDialog()
    DialogService-->>MainViewModel: filePath
    
    alt File Path Selected
        MainViewModel->>ImageSaveService: SaveImageAsync(image, filePath)
        ImageSaveService->>ImageSaveService: ConvertToTiff()
        ImageSaveService->>FileSystem: Write file
        FileSystem-->>ImageSaveService: Success
        ImageSaveService-->>MainViewModel: Success
        MainViewModel-->>MainWindow: Show Success Message
        MainWindow-->>User: "Image saved successfully"
    else Cancelled
        DialogService-->>MainViewModel: null
        MainViewModel-->>User: No action
    end
```

---

## Component Architecture

### Dependency Injection Container

```mermaid
graph TB
    subgraph "Application Startup"
        APP[App.xaml.cs]
        SC[ServiceCollection]
        SP[ServiceProvider]
    end
    
    subgraph "Registered Services"
        IIL[IImageLoaderService]
        IIS[IImageSaveService]
        IIF[IImageOperationFactory]
        IDS[IDialogService]
    end
    
    subgraph "ViewModels"
        MVM[MainViewModel]
    end
    
    subgraph "Views"
        MW[MainWindow]
    end
    
    APP --> SC
    SC --> IIL
    SC --> IIS
    SC --> IIF
    SC --> IDS
    SC --> MVM
    SC --> MW
    SC --> SP
    SP --> MW
    MW --> MVM
    MVM --> IIL
    MVM --> IIS
    MVM --> IIF
    MVM --> IDS
    
    style APP fill:#e1bee7
    style SC fill:#ce93d8
    style SP fill:#ba68c8
```

### Layer Dependencies

```mermaid
graph TD
    subgraph "Layer 1: Presentation"
        V[Views XAML]
        VM[ViewModels]
        C[Converters]
        CMD[Commands]
    end
    
    subgraph "Layer 2: Application"
        SVC[Services]
        FAC[Factories]
    end
    
    subgraph "Layer 3: Domain"
        OPS[Operations]
        MOD[Models]
        INT[Interfaces]
    end
    
    subgraph "Layer 4: Infrastructure"
        IO[File I/O]
        DLG[Dialogs]
    end
    
    V --> VM
    VM --> CMD
    VM --> SVC
    SVC --> FAC
    FAC --> OPS
    OPS --> MOD
    SVC --> INT
    OPS --> INT
    SVC --> IO
    SVC --> DLG
    
    style V fill:#ffebee
    style VM fill:#ffcdd2
    style SVC fill:#fff9c4
    style OPS fill:#f0f4c3
```

---

## Data Flow Diagrams

### Image Processing Pipeline

```mermaid
flowchart TD
    START([User Loads Image])
    LOAD[Load 16-bit TIFF File]
    CONVERT[Convert to ImageData]
    STATS[Calculate Statistics]
    DISPLAY1[Display Original]
    
    SELECT[User Selects Operation]
    PARAMS[Set Parameters]
    APPLY[Apply Operation]
    PROCESS[Process Image Data]
    RESULT[Generate Result]
    DISPLAY2[Display Processed]
    
    SAVE{Save Image?}
    EXPORT[Export as TIFF]
    END([Complete])
    
    START --> LOAD
    LOAD --> CONVERT
    CONVERT --> STATS
    STATS --> DISPLAY1
    DISPLAY1 --> SELECT
    SELECT --> PARAMS
    PARAMS --> APPLY
    APPLY --> PROCESS
    PROCESS --> RESULT
    RESULT --> DISPLAY2
    DISPLAY2 --> SAVE
    SAVE -->|Yes| EXPORT
    SAVE -->|No| END
    EXPORT --> END
    
    style START fill:#c8e6c9
    style PROCESS fill:#fff9c4
    style END fill:#ffccbc
```

### Operation Execution Flow

```mermaid
flowchart LR
    INPUT[Input Image]
    VALIDATE[Validate Parameters]
    ALLOC[Allocate Memory]
    ALGO[Run Algorithm]
    PROGRESS[Report Progress]
    CHECK{Cancelled?}
    OUTPUT[Output Image]
    CLEANUP[Cleanup Resources]
    
    INPUT --> VALIDATE
    VALIDATE --> ALLOC
    ALLOC --> ALGO
    ALGO --> PROGRESS
    PROGRESS --> CHECK
    CHECK -->|No| ALGO
    CHECK -->|Yes| CLEANUP
    ALGO -->|Complete| OUTPUT
    OUTPUT --> CLEANUP
    
    style INPUT fill:#e3f2fd
    style ALGO fill:#fff3e0
    style OUTPUT fill:#c8e6c9
```

---

## Deployment Architecture

### Application Structure

```mermaid
graph TB
    subgraph "Executable"
        EXE[ImageReviewerApp.exe]
        CONFIG[Configuration Files]
    end
    
    subgraph "Dependencies"
        NET[.NET 10 Runtime]
        WPF[WPF Framework]
        DI[Microsoft.Extensions.DependencyInjection]
    end
    
    subgraph "User Data"
        IMAGES[TIFF Images]
        OUTPUT[Processed Images]
    end
    
    EXE --> NET
    EXE --> WPF
    EXE --> DI
    EXE --> IMAGES
    EXE --> OUTPUT
    
    style EXE fill:#4fc3f7
    style NET fill:#81c784
```

### Folder Structure

```
ImageReviewerApp/
├── bin/
│   ├── ImageReviewerApp.exe          ← Main Executable
│   ├── ImageReviewerApp.dll          ← Application DLL
│   ├── ImageReviewerApp.deps.json   ← Dependencies
│   ├── ImageReviewerApp.runtimeconfig.json
│   └── Microsoft.Extensions.*.dll    ← Runtime Dependencies
├── src/
│   ├── Commands/
│   ├── Contracts/
│   ├── Converters/
│   ├── Enums/
│   ├── Extensions/
│   ├── Factories/
│   ├── Models/
│   ├── Operations/
│   ├── Services/
│   ├── Styles/
│   ├── Utilities/
│   ├── ViewModels/
│   ├── App.xaml
│   └── MainWindow.xaml
├── Properties/
│   └── launchSettings.json
└── app.manifest
```

---

## Design Patterns Applied

### 1. Model-View-ViewModel (MVVM)

```mermaid
graph LR
    V[View<br/>MainWindow.xaml] 
    VM[ViewModel<br/>MainViewModel]
    M[Model<br/>ImageData]
    
    V -->|Data Binding| VM
    V -->|Commands| VM
    VM -->|Property Changed| V
    VM -->|Updates| M
    M -->|Notifies| VM
    
    style V fill:#e3f2fd
    style VM fill:#fff9c4
    style M fill:#c8e6c9
```

### 2. Strategy Pattern

```mermaid
graph TB
    CLIENT[Client<br/>MainViewModel]
    CONTEXT[Context<br/>IImageOperation]
    
    STRAT1[ConcreteStrategy<br/>WindowLevelOperation]
    STRAT2[ConcreteStrategy<br/>GammaCorrectionOperation]
    STRAT3[ConcreteStrategy<br/>GaussianFilterOperation]
    
    CLIENT --> CONTEXT
    CONTEXT <|.. STRAT1
    CONTEXT <|.. STRAT2
    CONTEXT <|.. STRAT3
    
    style CONTEXT fill:#fff3e0
    style STRAT1 fill:#e0f2f1
    style STRAT2 fill:#e0f2f1
    style STRAT3 fill:#e0f2f1
```

### 3. Factory Pattern

```mermaid
graph LR
    CLIENT[Client]
    FACTORY[Factory<br/>ImageOperationFactory]
    
    PROD1[Product<br/>Operation 1]
    PROD2[Product<br/>Operation 2]
    PROD3[Product<br/>Operation 3]
    
    CLIENT -->|Request| FACTORY
    FACTORY -.->|Creates| PROD1
    FACTORY -.->|Creates| PROD2
    FACTORY -.->|Creates| PROD3
    
    style FACTORY fill:#ffecb3
```

### 4. Dependency Injection

```mermaid
graph TB
    DI[DI Container]
    INT1[IImageLoaderService]
    INT2[IImageSaveService]
    INT3[IImageOperationFactory]
    
    IMPL1[ImageLoaderService]
    IMPL2[ImageSaveService]
    IMPL3[ImageOperationFactory]
    
    VM[MainViewModel]
    
    DI --> INT1
    DI --> INT2
    DI --> INT3
    INT1 -.implements.-> IMPL1
    INT2 -.implements.-> IMPL2
    INT3 -.implements.-> IMPL3
    DI -->|Injects| VM
    VM --> INT1
    VM --> INT2
    VM --> INT3
    
    style DI fill:#ce93d8
    style VM fill:#fff9c4
```

---

## Performance Considerations

### Memory Management

```mermaid
graph TD
    LOAD[Load Image<br/>~4MB for 1024x1024]
    ORIG[Original Image<br/>Memory~ushort~]
    PROC[Processed Image<br/>Memory~ushort~]
    DISP1[Display Buffer 1<br/>8-bit RGB]
    DISP2[Display Buffer 2<br/>8-bit RGB]
    TEMP[Temp Buffers<br/>During Operations]
    
    LOAD --> ORIG
    LOAD --> DISP1
    ORIG --> PROC
    PROC --> DISP2
    PROC --> TEMP
    
    style ORIG fill:#c8e6c9
    style PROC fill:#fff9c4
    style TEMP fill:#ffccbc
```

### Async Processing Pipeline

```mermaid
graph LR
    UI[UI Thread]
    BG[Background Thread]
    PROG[Progress Reporter]
    CANCEL[Cancellation Token]
    
    UI -->|Start| BG
    BG -->|Update| PROG
    PROG -->|Notify| UI
    UI -->|Request| CANCEL
    CANCEL -->|Stop| BG
    BG -->|Complete| UI
    
    style UI fill:#e3f2fd
    style BG fill:#fff9c4
```

---

## Extension Points

### Adding New Operations

```mermaid
flowchart TD
    START([New Operation Required])
    INT[Implement IImageOperation]
    ENUM[Add to ImageOperation Enum]
    FAC[Register in Factory]
    PARAM[Create Parameters Class]
    PARAMVM[Create ParametersViewModel]
    TEMPLATE[Add DataTemplate]
    TEST[Write Unit Tests]
    END([Operation Ready])
    
    START --> INT
    INT --> ENUM
    ENUM --> FAC
    FAC --> PARAM
    PARAM --> PARAMVM
    PARAMVM --> TEMPLATE
    TEMPLATE --> TEST
    TEST --> END
    
    style START fill:#c8e6c9
    style END fill:#81c784
```

---

## Security Architecture

```mermaid
graph TB
    subgraph "Application Boundary"
        APP[Application]
        VAL[Input Validation]
        ERR[Error Handling]
    end
    
    subgraph "File System"
        FS[Local File System]
        TIFF[TIFF Files]
    end
    
    subgraph "Memory"
        MEM[Process Memory]
        SAFE[Memory-Safe Operations]
    end
    
    APP --> VAL
    VAL --> FS
    FS --> TIFF
    APP --> MEM
    MEM --> SAFE
    APP --> ERR
    
    style VAL fill:#ffecb3
    style ERR fill:#ffccbc
    style SAFE fill:#c8e6c9
```

---

## Testing Architecture

```mermaid
graph TB
    subgraph "Unit Tests"
        UT1[Command Tests]
        UT2[Operation Tests]
        UT3[ViewModel Tests]
        UT4[Model Tests]
    end
    
    subgraph "Integration Tests"
        IT1[Service Integration]
        IT2[Pipeline Tests]
    end
    
    subgraph "Test Infrastructure"
        XUNIT[xUnit Framework]
        MOQ[Moq - Mocking]
        FA[FluentAssertions]
    end
    
    UT1 --> XUNIT
    UT2 --> XUNIT
    UT3 --> MOQ
    UT4 --> FA
    IT1 --> XUNIT
    IT2 --> XUNIT
    
    style XUNIT fill:#e1bee7
    style MOQ fill:#ce93d8
```

---

## Conclusion

This architecture provides:
- ✅ **Separation of Concerns** - Clear layer boundaries
- ✅ **Testability** - 85% code coverage achievable
- ✅ **Maintainability** - SOLID principles applied
- ✅ **Extensibility** - Easy to add new operations
- ✅ **Performance** - Async operations, optimized algorithms
- ✅ **Scalability** - Supports larger images with optimization strategies

**Version:** 1.0.0  
**Last Updated:** June 2026  
**Author:** Anoopa Kedila
