using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using A = DocumentFormat.OpenXml.Drawing;
using P = DocumentFormat.OpenXml.Presentation;

public class LyricsPowerPointGenerator
{
    public void CreatePresentationFromLyrics(List<string> songLyrics, string outputPath, string presentationTitle = "Song Lyrics")
    {
        using (var presentationDocument = PresentationDocument.Create(outputPath, PresentationDocumentType.Presentation))
        {
            CreatePresentationParts(presentationDocument, songLyrics, presentationTitle);
        }
    }

    private void CreatePresentationParts(PresentationDocument presentationDocument, List<string> songLyrics, string title)
    {
        // Create the main presentation part
        var presentationPart = presentationDocument.AddPresentationPart();

        // Create theme part first
        var themePart = CreateThemePart(presentationPart);

        // Create slide master part
        var slideMasterPart = CreateSlideMasterPart(presentationPart, themePart);

        // Create slide layout part
        var slideLayoutPart = CreateSlideLayoutPart(slideMasterPart);

        // Create slides
        var slideIdList = new SlideIdList();
        uint slideId = 256;

        // Title slide
        var titleSlidePart = CreateTitleSlide(presentationPart, slideLayoutPart, title);
        var titleSlideId = new SlideId() { Id = slideId++, RelationshipId = presentationPart.GetIdOfPart(titleSlidePart) };
        slideIdList.Append(titleSlideId);

        // Lyrics slides
        for (int i = 0; i < songLyrics.Count; i++)
        {
            var slidePart = CreateLyricsSlide(presentationPart, slideLayoutPart, songLyrics[i], $"Song {i + 1}");
            var slideIdElement = new SlideId() { Id = slideId++, RelationshipId = presentationPart.GetIdOfPart(slidePart) };
            slideIdList.Append(slideIdElement);
        }

        // Create slide master ID list
        var slideMasterIdList = new SlideMasterIdList();
        var slideMasterId = new SlideMasterId()
        {
            Id = 2147483648U,
            RelationshipId = presentationPart.GetIdOfPart(slideMasterPart)
        };
        slideMasterIdList.Append(slideMasterId);

        // Create the presentation
        presentationPart.Presentation = new Presentation();
        presentationPart.Presentation.SlideMasterIdList = slideMasterIdList;
        presentationPart.Presentation.SlideIdList = slideIdList;
        presentationPart.Presentation.SlideSize = new SlideSize() { Cx = 9144000, Cy = 6858000, Type = SlideSizeValues.Screen16x9 };
        presentationPart.Presentation.NotesSize = new NotesSize() { Cx = 6858000, Cy = 9144000 };

        presentationPart.Presentation.Save();
    }

    private ThemePart CreateThemePart(PresentationPart presentationPart)
    {
        var themePart = presentationPart.AddNewPart<ThemePart>();

        var theme = new A.Theme() { Name = "Office Theme" };

        var themeElements = new A.ThemeElements();

        // Color scheme
        var colorScheme = new A.ColorScheme() { Name = "Office" };
        colorScheme.Append(new A.Dark1Color(new A.SystemColor() { Val = A.SystemColorValues.WindowText, LastColor = "000000" }));
        colorScheme.Append(new A.Light1Color(new A.SystemColor() { Val = A.SystemColorValues.Window, LastColor = "FFFFFF" }));
        colorScheme.Append(new A.Dark2Color(new A.RgbColorModelHex() { Val = "44546A" }));
        colorScheme.Append(new A.Light2Color(new A.RgbColorModelHex() { Val = "E7E6E6" }));
        colorScheme.Append(new A.Accent1Color(new A.RgbColorModelHex() { Val = "4472C4" }));
        colorScheme.Append(new A.Accent2Color(new A.RgbColorModelHex() { Val = "E15759" }));
        colorScheme.Append(new A.Accent3Color(new A.RgbColorModelHex() { Val = "A5A5A5" }));
        colorScheme.Append(new A.Accent4Color(new A.RgbColorModelHex() { Val = "FFC000" }));
        colorScheme.Append(new A.Accent5Color(new A.RgbColorModelHex() { Val = "5B9BD5" }));
        colorScheme.Append(new A.Accent6Color(new A.RgbColorModelHex() { Val = "70AD47" }));
        colorScheme.Append(new A.Hyperlink(new A.RgbColorModelHex() { Val = "0563C1" }));
        colorScheme.Append(new A.FollowedHyperlinkColor(new A.RgbColorModelHex() { Val = "954F72" }));

        // Font scheme
        var fontScheme = new A.FontScheme() { Name = "Office" };
        var majorFont = new A.MajorFont();
        majorFont.Append(new A.LatinFont() { Typeface = "Calibri Light" });
        majorFont.Append(new A.EastAsianFont() { Typeface = "" });
        majorFont.Append(new A.ComplexScriptFont() { Typeface = "" });

        var minorFont = new A.MinorFont();
        minorFont.Append(new A.LatinFont() { Typeface = "Calibri" });
        minorFont.Append(new A.EastAsianFont() { Typeface = "" });
        minorFont.Append(new A.ComplexScriptFont() { Typeface = "" });

        fontScheme.Append(majorFont);
        fontScheme.Append(minorFont);

        // Format scheme
        var formatScheme = new A.FormatScheme() { Name = "Office" };

        var fillStyleList = new A.FillStyleList();
        fillStyleList.Append(new A.SolidFill(new A.SchemeColor() { Val = A.SchemeColorValues.PhColor }));
        fillStyleList.Append(new A.GradientFill(
            new A.GradientStopList(
                new A.GradientStop(new A.SchemeColor() { Val = A.SchemeColorValues.PhColor }) { Position = 0 },
                new A.GradientStop(new A.SchemeColor() { Val = A.SchemeColorValues.PhColor }) { Position = 100000 }),
            new A.LinearGradientFill() { Angle = 5400000, Scaled = true }));
        fillStyleList.Append(new A.GradientFill(
            new A.GradientStopList(
                new A.GradientStop(new A.SchemeColor() { Val = A.SchemeColorValues.PhColor }) { Position = 0 },
                new A.GradientStop(new A.SchemeColor() { Val = A.SchemeColorValues.PhColor }) { Position = 100000 }),
            new A.LinearGradientFill() { Angle = 5400000, Scaled = true }));

        var lineStyleList = new A.LineStyleList();
        lineStyleList.Append(new A.Outline(new A.SolidFill(new A.SchemeColor() { Val = A.SchemeColorValues.PhColor })) { Width = 9525 });
        lineStyleList.Append(new A.Outline(new A.SolidFill(new A.SchemeColor() { Val = A.SchemeColorValues.PhColor })) { Width = 25400 });
        lineStyleList.Append(new A.Outline(new A.SolidFill(new A.SchemeColor() { Val = A.SchemeColorValues.PhColor })) { Width = 38100 });

        var effectStyleList = new A.EffectStyleList();
        effectStyleList.Append(new A.EffectStyle(new A.EffectList()));
        effectStyleList.Append(new A.EffectStyle(new A.EffectList()));
        effectStyleList.Append(new A.EffectStyle(new A.EffectList()));

        formatScheme.Append(fillStyleList);
        formatScheme.Append(lineStyleList);
        formatScheme.Append(effectStyleList);

        themeElements.Append(colorScheme);
        themeElements.Append(fontScheme);
        themeElements.Append(formatScheme);

        theme.Append(themeElements);
        themePart.Theme = theme;

        return themePart;
    }

    private SlideMasterPart CreateSlideMasterPart(PresentationPart presentationPart, ThemePart themePart)
    {
        var slideMasterPart = presentationPart.AddNewPart<SlideMasterPart>();
        slideMasterPart.AddPart(themePart);

        var slideMaster = new SlideMaster();

        var commonSlideData = new CommonSlideData();
        var shapeTree = new ShapeTree();

        var nonVisualGroupShapeProperties = new P.NonVisualGroupShapeProperties();
        nonVisualGroupShapeProperties.Append(new P.NonVisualDrawingProperties() { Id = 1U, Name = "" });
        nonVisualGroupShapeProperties.Append(new P.NonVisualGroupShapeDrawingProperties());
        nonVisualGroupShapeProperties.Append(new ApplicationNonVisualDrawingProperties());

        var groupShapeProperties = new GroupShapeProperties();
        groupShapeProperties.Append(new A.TransformGroup());

        shapeTree.Append(nonVisualGroupShapeProperties);
        shapeTree.Append(groupShapeProperties);

        commonSlideData.Append(shapeTree);

        var colorMap = new P.ColorMap()
        {
            Background1 = A.ColorSchemeIndexValues.Light1,
            Text1 = A.ColorSchemeIndexValues.Dark1,
            Background2 = A.ColorSchemeIndexValues.Light2,
            Text2 = A.ColorSchemeIndexValues.Dark2,
            Accent1 = A.ColorSchemeIndexValues.Accent1,
            Accent2 = A.ColorSchemeIndexValues.Accent2,
            Accent3 = A.ColorSchemeIndexValues.Accent3,
            Accent4 = A.ColorSchemeIndexValues.Accent4,
            Accent5 = A.ColorSchemeIndexValues.Accent5,
            Accent6 = A.ColorSchemeIndexValues.Accent6,
            Hyperlink = A.ColorSchemeIndexValues.Hyperlink,
            FollowedHyperlink = A.ColorSchemeIndexValues.FollowedHyperlink
        };

        var slideLayoutIdList = new SlideLayoutIdList();

        slideMaster.Append(commonSlideData);
        slideMaster.Append(colorMap);
        slideMaster.Append(slideLayoutIdList);

        slideMasterPart.SlideMaster = slideMaster;

        return slideMasterPart;
    }

    private SlideLayoutPart CreateSlideLayoutPart(SlideMasterPart slideMasterPart)
    {
        var slideLayoutPart = slideMasterPart.AddNewPart<SlideLayoutPart>();

        var slideLayout = new SlideLayout() { Type = SlideLayoutValues.Blank };

        var commonSlideData = new CommonSlideData();
        var shapeTree = new ShapeTree();

        var nonVisualGroupShapeProperties = new P.NonVisualGroupShapeProperties();
        nonVisualGroupShapeProperties.Append(new P.NonVisualDrawingProperties() { Id = 1U, Name = "" });
        nonVisualGroupShapeProperties.Append(new P.NonVisualGroupShapeDrawingProperties());
        nonVisualGroupShapeProperties.Append(new ApplicationNonVisualDrawingProperties());

        var groupShapeProperties = new GroupShapeProperties();
        groupShapeProperties.Append(new A.TransformGroup());

        shapeTree.Append(nonVisualGroupShapeProperties);
        shapeTree.Append(groupShapeProperties);

        commonSlideData.Append(shapeTree);

        var colorMapOverride = new P.ColorMapOverride();
        colorMapOverride.Append(new A.MasterColorMapping());

        slideLayout.Append(commonSlideData);
        slideLayout.Append(colorMapOverride);

        slideLayoutPart.SlideLayout = slideLayout;

        // Add to slide master's layout list
        var slideLayoutId = new SlideLayoutId()
        {
            Id = 2147483649U,
            RelationshipId = slideMasterPart.GetIdOfPart(slideLayoutPart)
        };
        slideMasterPart.SlideMaster.SlideLayoutIdList.Append(slideLayoutId);

        return slideLayoutPart;
    }

    private SlidePart CreateTitleSlide(PresentationPart presentationPart, SlideLayoutPart slideLayoutPart, string title)
    {
        var slidePart = presentationPart.AddNewPart<SlidePart>();
        slidePart.AddPart(slideLayoutPart);

        var slide = new Slide();
        var commonSlideData = new CommonSlideData();
        var shapeTree = new ShapeTree();

        var nonVisualGroupShapeProperties = new P.NonVisualGroupShapeProperties();
        nonVisualGroupShapeProperties.Append(new P.NonVisualDrawingProperties() { Id = 1U, Name = "" });
        nonVisualGroupShapeProperties.Append(new P.NonVisualGroupShapeDrawingProperties());
        nonVisualGroupShapeProperties.Append(new ApplicationNonVisualDrawingProperties());

        var groupShapeProperties = new GroupShapeProperties();
        groupShapeProperties.Append(new A.TransformGroup());

        shapeTree.Append(nonVisualGroupShapeProperties);
        shapeTree.Append(groupShapeProperties);

        // Add title text box
        var titleShape = CreateTextShape(title, 2U, 1524000, 1143000, 6858000, 1143000, 4400, true);
        shapeTree.Append(titleShape);

        commonSlideData.Append(shapeTree);
        slide.Append(commonSlideData);

        slidePart.Slide = slide;
        return slidePart;
    }

    private SlidePart CreateLyricsSlide(PresentationPart presentationPart, SlideLayoutPart slideLayoutPart, string lyrics, string slideTitle)
    {
        var slidePart = presentationPart.AddNewPart<SlidePart>();
        slidePart.AddPart(slideLayoutPart);

        var slide = new Slide();
        var commonSlideData = new CommonSlideData();
        var shapeTree = new ShapeTree();

        var nonVisualGroupShapeProperties = new P.NonVisualGroupShapeProperties();
        nonVisualGroupShapeProperties.Append(new P.NonVisualDrawingProperties() { Id = 1U, Name = "" });
        nonVisualGroupShapeProperties.Append(new P.NonVisualGroupShapeDrawingProperties());
        nonVisualGroupShapeProperties.Append(new ApplicationNonVisualDrawingProperties());

        var groupShapeProperties = new GroupShapeProperties();
        groupShapeProperties.Append(new A.TransformGroup());

        shapeTree.Append(nonVisualGroupShapeProperties);
        shapeTree.Append(groupShapeProperties);

        // Add title
        var titleShape = CreateTextShape(slideTitle, 2U, 1524000, 685800, 6858000, 914400, 3600, true);
        shapeTree.Append(titleShape);

        // Add lyrics
        var lyricsShape = CreateTextShape(lyrics, 3U, 1524000, 1828800, 6858000, 4572000, 2400, false);
        shapeTree.Append(lyricsShape);

        commonSlideData.Append(shapeTree);
        slide.Append(commonSlideData);

        slidePart.Slide = slide;
        return slidePart;
    }

    private P.Shape CreateTextShape(string text, uint shapeId, int x, int y, int width, int height, int fontSize, bool isBold)
    {
        var shape = new P.Shape();

        var nonVisualShapeProperties = new P.NonVisualShapeProperties();
        nonVisualShapeProperties.Append(new P.NonVisualDrawingProperties() { Id = shapeId, Name = $"TextBox {shapeId}" });
        nonVisualShapeProperties.Append(new P.NonVisualShapeDrawingProperties(new A.ShapeLocks() { NoGrouping = true }));
        nonVisualShapeProperties.Append(new ApplicationNonVisualDrawingProperties());

        var shapeProperties = new P.ShapeProperties();
        var transform2D = new A.Transform2D();
        transform2D.Append(new A.Offset() { X = x, Y = y });
        transform2D.Append(new A.Extents() { Cx = width, Cy = height });
        shapeProperties.Append(transform2D);

        var presetGeometry = new A.PresetGeometry() { Preset = A.ShapeTypeValues.Rectangle };
        presetGeometry.Append(new A.AdjustValueList());
        shapeProperties.Append(presetGeometry);

        var textBody = new P.TextBody();
        textBody.Append(new A.BodyProperties());
        textBody.Append(new A.ListStyle());

        var paragraph = new A.Paragraph();
        var paragraphProperties = new A.ParagraphProperties();
        paragraphProperties.Append(new A.DefaultRunProperties());
        paragraph.Append(paragraphProperties);

        // Handle multi-line text
        //var lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        text=text.Replace("\r","");
        var lines = text.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; i++)
        {
            if (i > 0)
            {
                paragraph.Append(new A.Break());
            }

            var run = new A.Run();
            var runProperties = new A.RunProperties() { FontSize = fontSize };
            if (isBold)
            {
                runProperties.Bold = true;
            }
            run.Append(runProperties);
            run.Append(new A.Text(lines[i]));
            paragraph.Append(run);
        }

        textBody.Append(paragraph);

        shape.Append(nonVisualShapeProperties);
        shape.Append(shapeProperties);
        shape.Append(textBody);

        return shape;
    }
}