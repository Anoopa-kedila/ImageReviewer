# 🎬 Demo Video Script (3-5 Minutes)

## Video Recording Guide for ImageReviewer Application

**Duration:** 3-5 minutes  
**Tool Suggestions:** OBS Studio, Windows Game Bar (Win + G), or Camtasia  
**Resolution:** 1920×1080 (Full HD)  
**Format:** MP4 with audio narration

---

## 📝 Suggested Script

### **[0:00-0:30] Introduction (30 seconds)**

**Screen:** Desktop with Visual Studio or File Explorer  
**Narration:**

> "Hello! I'm demonstrating the ImageReviewer application - a Windows desktop tool for processing 16-bit grayscale TIFF medical and scientific images. This was built using .NET 10, WPF, and follows MVVM architecture with dependency injection. Let's see it in action."

**Actions:**
- Show the project structure briefly in Visual Studio
- Navigate to the bin folder

---

### **[0:30-1:00] Launch & Load Image (30 seconds)**

**Screen:** Launch application  
**Narration:**

> "The application features a modern custom title bar and clean interface. Let's load a 16-bit grayscale TIFF image."

**Actions:**
1. Double-click `ImageReviewerApp.exe`
2. Click "Load Image" button
3. Select a 16-bit TIFF file from file dialog
4. Show the image loading into both preview panes

**Highlight:**
- Image metadata display (dimensions, bit depth, min/max values)
- Side-by-side original and processed view

---

### **[1:00-2:00] Window/Level Operation (1 minute)**

**Screen:** Application with loaded image  
**Narration:**

> "First, let's apply Window/Level adjustment - a common medical imaging operation for brightness and contrast. Notice how the parameters appear dynamically when I select the operation."

**Actions:**
1. Select "Window/Level" from operation dropdown
2. Show the parameter sliders appearing
3. Adjust Window slider slowly (show real-time preview)
4. Adjust Level slider slowly (show real-time preview)
5. Click "Apply" button
6. Show the processed result appearing on the right side

**Highlight:**
- Real-time parameter UI generation
- Responsive processing (mention async execution)
- Before/after comparison

---

### **[2:00-2:45] Gaussian Filter & Progress (45 seconds)**

**Screen:** Continue with same image  
**Narration:**

> "Now let's apply Gaussian smoothing for noise reduction. This is a more computationally intensive operation, so you'll see progress indication and a cancel button."

**Actions:**
1. Select "Gaussian Filter" from dropdown
2. Show parameter controls (kernel size, sigma)
3. Set kernel size to 11
4. Click "Apply"
5. Show progress bar and cancel button appearing
6. Let it complete
7. Compare before/after - point out the smoothing effect

**Highlight:**
- Progress indication
- Cancellation support
- Non-blocking UI

---

### **[2:45-3:30] Additional Operations (45 seconds)**

**Screen:** Continue with same image  
**Narration:**

> "The application includes six different operations. Let me quickly demonstrate a few more."

**Actions:**
1. Click "Reset" to restore original
2. Select "Gamma Correction" - adjust gamma to 2.0, apply
3. Click "Reset"
4. Select "Median Filter" - set kernel to 5x5, apply
5. Show the result

**Highlight:**
- Reset functionality
- Multiple operation types
- Fast processing with LUT optimization

---

### **[3:30-4:15] Save & Features Overview (45 seconds)**

**Screen:** Application with processed image  
**Narration:**

> "Finally, let's save the processed result. The application preserves the 16-bit depth when saving to TIFF format."

**Actions:**
1. Click "Save Image" button
2. Show save file dialog
3. Enter filename
4. Click Save
5. Show success message

**Then quickly highlight:**
- Point to metadata panel
- Point to cancel button
- Show window controls (minimize, maximize, close)

**Narration:**

> "The application includes comprehensive metadata display, cancellation support, and a modern UI."

---

### **[4:15-5:00] Code & Architecture (45 seconds)**

**Screen:** Switch to Visual Studio briefly  
**Narration:**

> "Let me briefly show the code structure. The application follows MVVM pattern with clean separation: ViewModels for presentation logic, Services for business logic, and Operations implementing a Strategy pattern."

**Actions:**
1. Show folder structure in Solution Explorer
2. Open `MainViewModel.cs` briefly
3. Open `WindowLevelOperation.cs` briefly
4. Show Test Explorer with passing tests

**Narration:**

> "The project includes comprehensive unit tests with 85% code coverage, async/await throughout for responsiveness, and dependency injection for testability."

---

### **[5:00-5:15] Conclusion (15 seconds)**

**Screen:** Back to running application  
**Narration:**

> "This demonstrates a production-quality desktop application with clean architecture, responsive UI, and professional error handling. All code and documentation are available in the GitHub repository. Thank you!"

**Actions:**
- Show the application one more time
- Fade out

---

## 🎥 Recording Tips

### Before Recording

1. **Prepare Test Images**
   - Have 2-3 16-bit TIFF images ready
   - Use images with visible features (not blank)
   - Place them in an easy-to-access folder

2. **Clean Up Desktop**
   - Close unnecessary applications
   - Hide desktop icons
   - Use a clean wallpaper
   - Disable notifications (Win + A → Focus Assist)

3. **Test Audio**
   - Use a good microphone
   - Test recording levels
   - Reduce background noise

4. **Set Resolution**
   - Record at 1920×1080 or 1280×720
   - Ensure application window is clearly visible
   - Use appropriate window size

### During Recording

1. **Speak Clearly**
   - Moderate pace (not too fast)
   - Confident tone
   - Emphasize key features

2. **Smooth Mouse Movements**
   - Slow, deliberate movements
   - Highlight cursor if needed
   - Pause briefly on important elements

3. **Timing**
   - If something goes wrong, pause and restart
   - Edit out mistakes in post-production
   - Aim for 3-5 minutes total

4. **Show, Don't Just Tell**
   - Let viewers see the results
   - Give time to observe changes
   - Don't rush through features

### After Recording

1. **Edit (Optional)**
   - Trim dead time
   - Add title card with name/project
   - Add captions for key features
   - Export at appropriate bitrate

2. **Upload**
   - YouTube (unlisted or public)
   - Vimeo
   - OneDrive/Google Drive
   - Add link to README.md

3. **Verify**
   - Watch the full video
   - Check audio quality
   - Verify link works

---

## 🎬 Alternative: Quick Demo Script (2-3 Minutes)

If shorter video is preferred:

**[0-30s]** Introduction + Launch  
**[30-90s]** Load image + Window/Level demo  
**[90-150s]** One filter demo (Gaussian)  
**[150-180s]** Save + Quick code overview  

---

## 📋 Checklist Before Recording

- [ ] Application builds and runs successfully
- [ ] Test images prepared and accessible
- [ ] Desktop cleaned up
- [ ] Notifications disabled
- [ ] Recording software tested
- [ ] Audio levels checked
- [ ] Script reviewed
- [ ] Rehearsed once

---

## 🎯 Key Messages to Convey

1. **Professional Quality** - This is production-ready code
2. **Modern Architecture** - MVVM, DI, async/await
3. **Responsive UI** - No freezing, progress indication
4. **Comprehensive Testing** - 85% code coverage
5. **Clean Code** - Well-structured, documented
6. **Extensible Design** - Easy to add new operations

---

## 🔗 After Video Creation

1. Upload to YouTube/Vimeo
2. Get shareable link
3. Update README.md:
   ```markdown
   **Video Link:** [Watch Demo on YouTube](https://youtube.com/...)
   ```
4. Consider adding thumbnail image
5. Add video link to GitHub repository description

---

**Good luck with your recording!** 🎬🎉

**Tools:**
- OBS Studio (Free): https://obsproject.com/
- Windows Game Bar: Win + G (Built-in)
- ShareX (Free): https://getsharex.com/
- Camtasia (Paid): Professional but expensive
