# Quick Start Guide - ImageReviewer

## 🚀 Get Up and Running in 5 Minutes

### Step 1: Prerequisites
```bash
# Check if .NET 10 is installed
dotnet --version
# Should show: 10.0.xxx
```

If not installed: [Download .NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

---

### Step 2: Clone and Build
```bash
# Clone the repository
git clone https://github.com/Anoopa-kedila/ImageReviewer.git
cd ImageReviewer

# Restore and build
dotnet restore
dotnet build --configuration Release

# Run the application
dotnet run --project ImageReviewerApp/ImageReviewerApp.csproj
```

---

### Step 3: Run the Application

**Option A: Using Visual Studio**
1. Open `ImageReviewerApp.slnx`
2. Press **F5**

**Option B: Run the executable**
```bash
.\ImageReviewerApp\bin\ImageReviewerApp.exe
```

---

### Step 4: Process Your First Image

1. **Click "Load Image"** → Select a 16-bit grayscale TIFF file
2. **Choose an operation** from the dropdown (e.g., "Window/Level")
3. **Adjust parameters** using the sliders
4. **Click "Apply"** to process
5. **Click "Save Image"** to export the result

---

## 📚 Documentation Files

| File | Purpose |
|------|---------|
| **README.md** | Complete documentation (tech stack, architecture, features) |
| **FUTURE_IMPROVEMENTS.md** | Detailed improvement plan for next 1-2 days |
| **PROJECT_CONFIGURATION.md** | Build configuration and troubleshooting |
| **QuickStart.md** | This file - get started fast |

---

## 🎯 Key Features at a Glance

### Image Processing Operations
- ✅ **Window/Level** - Adjust brightness and contrast
- ✅ **Gamma Correction** - Non-linear brightness
- ✅ **Gaussian Filter** - Smooth noise reduction
- ✅ **Median Filter** - Salt-and-pepper noise removal
- ✅ **Thresholding** - Binary segmentation
- ✅ **Bad Pixel Suppression** - Detect and correct anomalies

### User Interface
- ✅ Side-by-side original vs. processed view
- ✅ Real-time parameter adjustment
- ✅ Progress indication
- ✅ Cancel long-running operations
- ✅ Reset to original
- ✅ Image metadata display

---

## 🧪 Run Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"
```

---

## 🐛 Troubleshooting

### Build Errors
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

### Missing .NET 10
Install the .NET 10 SDK from: https://dotnet.microsoft.com/download/dotnet/10.0

### Application Won't Start
1. Check that all files are in `ImageReviewerApp\bin\`
2. Verify Windows 10/11 operating system
3. Check Output window in Visual Studio for errors

---

## 💡 Sample Images

For testing, use 16-bit grayscale TIFF images. Common sources:
- Medical imaging DICOM exports (converted to TIFF)
- Scientific imaging cameras
- Microscopy image stacks
- Astronomy telescope data

---

## 📞 Need Help?

1. Check `PROJECT_CONFIGURATION.md` for detailed configuration
2. Review `README.md` for architecture and features
3. Examine the unit tests for usage examples
4. Check code comments for implementation details

---

## 🎓 Learning Path

### New to WPF?
1. Start with `App.xaml.cs` to see DI setup
2. Review `MainWindow.xaml` for UI layout
3. Study `MainViewModel.cs` for MVVM pattern
4. Check `Commands/` folder for command pattern

### New to Image Processing?
1. Start with `WindowLevelOperation.cs` (simplest)
2. Review `GammaCorrectionOperation.cs` (LUT example)
3. Study `GaussianFilterOperation.cs` (convolution)
4. Examine `BadPixelSuppressionOperation.cs` (complex logic)

### Want to Add a New Operation?
1. Create new class implementing `IImageOperation`
2. Add to `ImageOperation` enum
3. Update `ImageOperationFactory`
4. Create parameter ViewModel if needed
5. Add data template to `OperationParameterTemplates.xaml`
6. Write unit tests

---

**Version:** 1.0.0  
**Last Updated:** June 2026  
**Author:** Anoopa Kedila
