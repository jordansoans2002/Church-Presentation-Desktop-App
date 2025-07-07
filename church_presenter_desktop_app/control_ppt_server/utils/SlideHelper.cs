using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using D = DocumentFormat.OpenXml.Drawing;
using P = DocumentFormat.OpenXml.Presentation;

public static class SlideHelper
{
    public static void AddSlideWithTextBox(PresentationDocument presentationDocument, string titleText, string bodyText)
    {
        // Get the presentation part
        var presentationPart = presentationDocument.PresentationPart;
        if (presentationPart == null)
            throw new InvalidOperationException("Presentation part not found");

        // Get the first slide layout (assuming it exists from your PowerPointUtils)
        var slideLayoutPart = presentationPart.SlideMasterParts.First().SlideLayoutParts.First();

        // Create a new slide part
        var slidePart = presentationPart.AddNewPart<SlidePart>();
        slidePart.AddPart(slideLayoutPart);

        // Create the slide
        var slide = new Slide();
        var commonSlideData = new CommonSlideData();
        var shapeTree = new ShapeTree();

        // Add required group shape properties
        shapeTree.NonVisualGroupShapeProperties = new NonVisualGroupShapeProperties(
            new NonVisualDrawingProperties { Id = 1, Name = "" },
            new NonVisualGroupShapeDrawingProperties(),
            new ApplicationNonVisualDrawingProperties());

        shapeTree.GroupShapeProperties = new GroupShapeProperties(
            new D.TransformGroup(
                new D.Offset { X = 0, Y = 0 },
                new D.Extents { Cx = 0, Cy = 0 },
                new D.ChildOffset { X = 0, Y = 0 },
                new D.ChildExtents { Cx = 0, Cy = 0 }));

        // Create title text box
        if (!string.IsNullOrEmpty(titleText))
        {
            var titleShape = CreateTextBox(2, titleText, 914400, 457200, 10972800, 1371600, true);
            shapeTree.Append(titleShape);
        }

        // Create body text box
        if (!string.IsNullOrEmpty(bodyText))
        {
            var bodyShape = CreateTextBox(3, bodyText, 914400, 2057400, 10972800, 4343400, false);
            shapeTree.Append(bodyShape);
        }

        // Add shape tree to slide
        commonSlideData.ShapeTree = shapeTree;
        slide.CommonSlideData = commonSlideData;
        slidePart.Slide = slide;

        // Add slide to presentation
        AddSlideToPresentation(presentationPart, slidePart);
    }

    private static P.Shape CreateTextBox(uint shapeId, string text, long x, long y, long width, long height, bool isTitle)
    {
        var shape = new P.Shape();

        // Non-visual properties
        shape.NonVisualShapeProperties = new NonVisualShapeProperties(
            new NonVisualDrawingProperties { Id = shapeId, Name = $"TextBox {shapeId}" },
            new NonVisualShapeDrawingProperties { TextBox = true },
            new ApplicationNonVisualDrawingProperties());

        // Shape properties
        shape.ShapeProperties = new ShapeProperties(
            new D.Transform2D(
                new D.Offset { X = x, Y = y },
                new D.Extents { Cx = width, Cy = height }),
            new D.PresetGeometry { Preset = D.ShapeTypeValues.Rectangle });

        // Text body
        var textBody = new TextBody(
            new D.BodyProperties
            {
                Wrap = D.TextWrappingValues.Square,
                LeftInset = 91440,
                TopInset = 45720,
                RightInset = 91440,
                BottomInset = 45720
            },
            new D.ListStyle());

        var paragraph = new D.Paragraph();

        // Set alignment
        if (isTitle)
        {
            paragraph.ParagraphProperties = new D.ParagraphProperties
            {
                Alignment = D.TextAlignmentTypeValues.Center
            };
        }

        // Handle line breaks in text
        var lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.None);
        for (int i = 0; i < lines.Length; i++)
        {
            if (i > 0)
            {
                paragraph.Append(new D.Break());
            }

            if (!string.IsNullOrEmpty(lines[i]))
            {
                var run = new D.Run(
                    new D.RunProperties
                    {
                        Language = "en-US",
                        FontSize = isTitle ? 3200 : 2000,
                        Bold = isTitle
                    },
                    new D.Text(lines[i]));
                paragraph.Append(run);
            }
        }

        textBody.Append(paragraph);
        shape.TextBody = textBody;

        return shape;
    }

    private static void AddSlideToPresentation(PresentationPart presentationPart, SlidePart slidePart)
    {
        // Get or create slide ID list
        var slideIdList = presentationPart.Presentation.SlideIdList;
        if (slideIdList == null)
        {
            slideIdList = new SlideIdList();
            presentationPart.Presentation.SlideIdList = slideIdList;
        }

        // Find the highest existing slide ID
        uint maxSlideId = 256;
        if (slideIdList.HasChildren)
        {
            maxSlideId = slideIdList.Elements<SlideId>().Max(s => s.Id?.Value ?? 256) + 1;
        }

        // Create new slide ID
        var slideId = new SlideId
        {
            Id = maxSlideId,
            RelationshipId = presentationPart.GetIdOfPart(slidePart)
        };

        slideIdList.Append(slideId);
    }
}