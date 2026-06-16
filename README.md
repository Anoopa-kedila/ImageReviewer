# ImageReviewer

A Windows desktop application for processing 16-bit grayscale TIFF medical and scientific images.

## Tech Stack

- **.NET 10** with C# 12
- **WPF** - Windows Presentation Foundation
- **MVVM Architecture** with Dependency Injection
- **xUnit + Moq** for testing (85% coverage)


## Features Implemented

### Image Operations (6 of 4 required)
1. **Window/Level** - Brightness and contrast
2. **Gamma Correction** - Non-linear brightness
3. **Gaussian Filter** - Noise reduction
4. **Median Filter** - Salt-and-pepper noise removal
5. **Thresholding** - Binary segmentation
6. **Bad Pixel Suppression** - Anomaly correction

### User Interface
- Load 16-bit TIFF images
- Side-by-side comparison
- Real-time parameters
- Metadata display
- Progress & cancellation
- Reset function
- Save as png


## Architecture

**MVVM Pattern** with Dependency Injection  
**Design Patterns:** Strategy, Factory, Async/Await

📐 **Detailed Architecture Documentation:** See [ARCHITECTURE.md](ARCHITECTURE.md) for:
- High-Level Design (HLD)
- Low-Level Design (LLD)
- Class Diagrams
- Sequence Diagrams
- Component Architecture
- Data Flow Diagrams


## Build & Run

### Prerequisites
- Windows 10/11
- .NET 10 SDK - [Download](https://dotnet.microsoft.com/download/dotnet/10.0)

### Commands
```bash
git clone https://github.com/Anoopa-kedila/ImageReviewer.git
cd ImageReviewer
dotnet build
dotnet run --project ImageReviewerApp/ImageReviewerApp.csproj
```

Or run: `.\ImageReviewerApp\bin\ImageReviewerApp.exe`


## Testing

```bash
dotnet test
```

**Coverage:** 85%


## Requirements Analysis

See **REQUIREMENTS_ANALYSIS.xlsx** (Excel file) for detailed functional and non-functional requirements.

**Requirements Traceability Matrix includes:**
- 23 Functional Requirements (18 ✅ implemented, 5 ⏳ future)
- 20 Non-Functional Requirements (17 ✅ met, 2 ⚠️ partial, 1 N/A)
- 7 Constraints (all documented)
- 5 Assumptions (all documented)

**Excel Features:**
- Color-coded status (Green=Implemented, Red=Future, Yellow=Partial)
- Auto-filter enabled for easy searching
- Frozen header row
- Formatted columns with borders

## Assumptions

1. Input: 16-bit grayscale TIFF images
2. Image size: 512×512 to 2048×2048 pixels
3. Single image processing (no batch)
4. Windows platform only
5. Users have basic image processing knowledge


## Trade-offs

**Performance vs. Accuracy**  
- Used LUT for gamma and window/level  
- 10x faster but slightly less flexible

**Memory vs. Speed**  
- Keep both images in memory  
- Instant comparison but ~16MB extra

**Responsiveness vs. Complexity**  
- Async/await throughout  
- Non-blocking UI but more complex code



## Future Improvements (1-2 Days)

1. **Histogram Display** (4-6h)
2. **Undo/Redo** (3-4h)
3. **Zoom & Pan** (3-4h)
4. **Batch Processing** (4-5h)
5. **Region of Interest (ROI)** (2-3h)
   - Rectangular selection for ROI
   - Display ROI statistics such as min, max, mean, and standard deviation


## Author

**Anoopa Kedila**  
GitHub: [@Anoopa-kedila](https://github.com/Anoopa-kedila)


**Version:** 1.0.0 | **Status:** ✅ Passing | **Coverage:** 85%
