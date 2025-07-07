using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml;
using A = DocumentFormat.OpenXml.Drawing;
using P = DocumentFormat.OpenXml.Presentation;

namespace control_ppt_server.utils
{
    public class GeneratePowerPoint
    {
        public static byte[] CreatePowerPointPresentation(List<string> songs)
        {
            using var memoryStream = new MemoryStream();

            // Create the presentation document
            using (var presentationDocument = PresentationDocument.Create(memoryStream, PresentationDocumentType.Presentation))
            {
                // Create the main presentation part
                var presentationPart = presentationDocument.AddPresentationPart();

                // Create slide master and layout parts
                var slideMasterPart = CreateSlideMasterPart(presentationPart);
                var slideLayoutPart = CreateSlideLayoutPart(slideMasterPart);

                // Create the main presentation with proper structure
                var presentation = new Presentation();

                // Add slide size (standard 16:9 presentation)
                var slideSize = new SlideSize()
                {
                    Cx = 12192000, // Width: 13.33 inches
                    Cy = 6858000   // Height: 7.5 inches
                };
                presentation.SlideSize = slideSize;

                // Add notes size
                var notesSize = new NotesSize()
                {
                    Cx = 12192000,
                    Cy = 9144000
                };
                presentation.NotesSize = notesSize;

                // Add default text styles
                var defaultTextStyle = new DefaultTextStyle();
                presentation.DefaultTextStyle = defaultTextStyle;

                // Create slides for each song
                var slideIdList = new SlideIdList();
                uint slideId = 256;

                foreach (var song in songs)
                {
                    var slidePart = CreateSlidePart(presentationPart, slideLayoutPart);

                    var slideIdEntry = new SlideId()
                    {
                        Id = slideId++,
                        RelationshipId = presentationPart.GetIdOfPart(slidePart)
                    };
                    slideIdList.Append(slideIdEntry);
                }

                // Add slide master ID list
                var slideMasterIdList = new SlideMasterIdList();
                var slideMasterId = new SlideMasterId()
                {
                    Id = 2147483648,
                    RelationshipId = presentationPart.GetIdOfPart(slideMasterPart)
                };
                slideMasterIdList.Append(slideMasterId);

                // Add all parts to presentation
                presentation.SlideMasterIdList = slideMasterIdList;
                presentation.SlideIdList = slideIdList;

                presentationPart.Presentation = presentation;
                presentationPart.Presentation.Save();
            }

            return memoryStream.ToArray();
        }

        private static SlideMasterPart CreateSlideMasterPart(PresentationPart presentationPart)
        {
            var slideMasterPart = presentationPart.AddNewPart<SlideMasterPart>();

            // Create theme part first
            var themePart = slideMasterPart.AddNewPart<ThemePart>();
            CreateTheme(themePart);

            var slideMaster = new SlideMaster();

            // Create common slide data with proper shape tree
            var commonSlideData = new CommonSlideData()
            {
                ShapeTree = new ShapeTree()
                {
                    NonVisualGroupShapeProperties = new NonVisualGroupShapeProperties()
                    {
                        NonVisualDrawingProperties = new NonVisualDrawingProperties() { Id = 1, Name = "" },
                        NonVisualGroupShapeDrawingProperties = new NonVisualGroupShapeDrawingProperties(),
                        ApplicationNonVisualDrawingProperties = new ApplicationNonVisualDrawingProperties()
                    },
                    GroupShapeProperties = new GroupShapeProperties()
                    {
                        TransformGroup = new A.TransformGroup()
                        {
                            Offset = new A.Offset() { X = 0, Y = 0 },
                            Extents = new A.Extents() { Cx = 0, Cy = 0 },
                            ChildOffset = new A.ChildOffset() { X = 0, Y = 0 },
                            ChildExtents = new A.ChildExtents() { Cx = 0, Cy = 0 }
                        }
                    }
                }
            };

            slideMaster.CommonSlideData = commonSlideData;

            // Add color map
            slideMaster.ColorMap = new ColorMap()
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

            // Add slide layout ID list (will be populated when layout is created)
            slideMaster.SlideLayoutIdList = new SlideLayoutIdList();

            slideMasterPart.SlideMaster = slideMaster;
            return slideMasterPart;
        }

        private static SlideLayoutPart CreateSlideLayoutPart(SlideMasterPart slideMasterPart)
        {
            var slideLayoutPart = slideMasterPart.AddNewPart<SlideLayoutPart>();
            var slideLayout = new SlideLayout() { Type = SlideLayoutValues.Blank };

            var commonSlideData = new CommonSlideData()
            {
                ShapeTree = new ShapeTree()
                {
                    NonVisualGroupShapeProperties = new NonVisualGroupShapeProperties()
                    {
                        NonVisualDrawingProperties = new NonVisualDrawingProperties() { Id = 1, Name = "" },
                        NonVisualGroupShapeDrawingProperties = new NonVisualGroupShapeDrawingProperties(),
                        ApplicationNonVisualDrawingProperties = new ApplicationNonVisualDrawingProperties()
                    },
                    GroupShapeProperties = new GroupShapeProperties()
                    {
                        TransformGroup = new A.TransformGroup()
                        {
                            Offset = new A.Offset() { X = 0, Y = 0 },
                            Extents = new A.Extents() { Cx = 0, Cy = 0 },
                            ChildOffset = new A.ChildOffset() { X = 0, Y = 0 },
                            ChildExtents = new A.ChildExtents() { Cx = 0, Cy = 0 }
                        }
                    }
                }
            };

            slideLayout.CommonSlideData = commonSlideData;
            slideLayoutPart.SlideLayout = slideLayout;

            // Add layout to slide master's layout list
            var slideLayoutId = new SlideLayoutId()
            {
                Id = 2147483649,
                RelationshipId = slideMasterPart.GetIdOfPart(slideLayoutPart)
            };
            slideMasterPart.SlideMaster.SlideLayoutIdList!.Append(slideLayoutId);

            return slideLayoutPart;
        }

        private static SlidePart CreateSlidePart(PresentationPart presentationPart, SlideLayoutPart slideLayoutPart)
        {
            var slidePart = presentationPart.AddNewPart<SlidePart>();
            slidePart.AddPart(slideLayoutPart);

            var slide = new Slide();
            var commonSlideData = new CommonSlideData();
            var shapeTree = new ShapeTree();

            // Non-visual group shape properties
            var nonVisualGroupShapeProperties = new NonVisualGroupShapeProperties()
            {
                NonVisualDrawingProperties = new NonVisualDrawingProperties() { Id = 1, Name = "" },
                NonVisualGroupShapeDrawingProperties = new NonVisualGroupShapeDrawingProperties(),
                ApplicationNonVisualDrawingProperties = new ApplicationNonVisualDrawingProperties()
            };
            shapeTree.NonVisualGroupShapeProperties = nonVisualGroupShapeProperties;

            // Group shape properties
            var groupShapeProperties = new GroupShapeProperties()
            {
                TransformGroup = new A.TransformGroup()
                {
                    Offset = new A.Offset() { X = 0, Y = 0 },
                    Extents = new A.Extents() { Cx = 0, Cy = 0 },
                    ChildOffset = new A.ChildOffset() { X = 0, Y = 0 },
                    ChildExtents = new A.ChildExtents() { Cx = 0, Cy = 0 }
                }
            };
            shapeTree.GroupShapeProperties = groupShapeProperties;

            // Create title text box
            var titleShape = CreateTextShape(2, "Title",
                914400, 457200, 10972800, 1371600, true); // Title position and size
            shapeTree.Append(titleShape);

            // Create lyrics text box
            var lyricsShape = CreateTextShape(3, "Lyrics",
                914400, 2057400, 10972800, 4343400, false); // Lyrics position and size
            shapeTree.Append(lyricsShape);

            commonSlideData.ShapeTree = shapeTree;
            slide.CommonSlideData = commonSlideData;

            slidePart.Slide = slide;
            return slidePart;
        }

        private static P.Shape CreateTextShape(uint shapeId, string text, long x, long y, long width, long height, bool isTitle)
        {
            var shape = new P.Shape();

            // Non-visual shape properties
            var nonVisualShapeProperties = new NonVisualShapeProperties()
            {
                NonVisualDrawingProperties = new NonVisualDrawingProperties()
                {
                    Id = shapeId,
                    Name = $"TextBox {shapeId}"
                },
                NonVisualShapeDrawingProperties = new NonVisualShapeDrawingProperties()
                {
                    TextBox = true
                },
                ApplicationNonVisualDrawingProperties = new ApplicationNonVisualDrawingProperties()
            };
            shape.NonVisualShapeProperties = nonVisualShapeProperties;

            // Shape properties
            var shapeProperties = new ShapeProperties()
            {
                Transform2D = new A.Transform2D()
                {
                    Offset = new A.Offset() { X = x, Y = y },
                    Extents = new A.Extents() { Cx = width, Cy = height }
                },
                //PresetGeometry = new D.PresetGeometry()
                //{
                //    Preset = D.ShapeTypeValues.Rectangle,
                //    AdjustValueList = new D.AdjustValueList()
                //},
                // Add a no-fill and no-line to make the text box invisible
                //NoFill = new D.NoFill(),
                //Outline = new D.Outline()
                //{
                //    Width = 0,
                //    NoFill = new D.NoFill()
                //}
            };

            //var transform2D = new D.Transform2D();
            //transform2D.Append(new D.Offset() { X = x, Y = y });
            //transform2D.Append(new D.Extents() { Cx = width, Cy = height });
            //shapeProperties.Append(transform2D);

            var presetGeometry = new A.PresetGeometry() { Preset = A.ShapeTypeValues.Rectangle };
            presetGeometry.Append(new A.AdjustValueList());
            shapeProperties.Append(presetGeometry);

            var noFill = new A.NoFill();
            shapeProperties.Append(noFill);

            var outline = new A.Outline();
            outline.Width = 0;
            outline.Append(new A.NoFill());
            shapeProperties.Append(outline);

            shape.ShapeProperties = shapeProperties;

            // Text body
            var textBody = new TextBody()
            {
                BodyProperties = new A.BodyProperties()
                {
                    Wrap = A.TextWrappingValues.Square,
                    LeftInset = 91440,   // 0.1 inch margins
                    TopInset = 45720,    // 0.05 inch margins
                    RightInset = 91440,
                    BottomInset = 45720,
                    Anchor = A.TextAnchoringTypeValues.Top,
                    AnchorCenter = false
                },
                ListStyle = new A.ListStyle()
            };

            var paragraph = new A.Paragraph();

            // Paragraph properties
            var paragraphProperties = new A.ParagraphProperties()
            {
                Alignment = isTitle ? A.TextAlignmentTypeValues.Center : A.TextAlignmentTypeValues.Left
            };
            paragraph.ParagraphProperties = paragraphProperties;

            // Split text into lines and create runs
            var lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                if (i > 0)
                {
                    // Add line break
                    paragraph.Append(new A.Break());
                }

                if (!string.IsNullOrEmpty(lines[i]))
                {
                    var run = new A.Run()
                    {
                        RunProperties = new A.RunProperties()
                        {
                            Language = "en-US",
                            FontSize = isTitle ? 3200 : 2000, // Larger font for title
                            Bold = isTitle,
                            Dirty = false
                        },
                        Text = new A.Text() { Text = lines[i] }
                    };
                    paragraph.Append(run);
                }
            }

            // Add end paragraph run properties
            paragraph.Append(new A.EndParagraphRunProperties()
            {
                Language = "en-US",
                FontSize = isTitle ? 3200 : 2000,
                Dirty = false
            });

            textBody.Append(paragraph);
            shape.TextBody = textBody;

            return shape;
        }

        private static void CreateTheme(ThemePart themePart)
        {
            var theme = new A.Theme() { Name = "Office Theme" };

            var themeElements = new A.ThemeElements();

            // Color scheme
            var colorScheme = new A.ColorScheme() { Name = "Office" };
            colorScheme.Append(new A.Dark1Color() { SystemColor = new A.SystemColor() { Val = A.SystemColorValues.WindowText } });
            colorScheme.Append(new A.Light1Color() { SystemColor = new A.SystemColor() { Val = A.SystemColorValues.Window } });
            colorScheme.Append(new A.Dark2Color() { RgbColorModelHex = new A.RgbColorModelHex() { Val = "1F497D" } });
            colorScheme.Append(new A.Light2Color() { RgbColorModelHex = new A.RgbColorModelHex() { Val = "EEECE1" } });
            colorScheme.Append(new A.Accent1Color() { RgbColorModelHex = new A.RgbColorModelHex() { Val = "4F81BD" } });
            colorScheme.Append(new A.Accent2Color() { RgbColorModelHex = new A.RgbColorModelHex() { Val = "F79646" } });
            colorScheme.Append(new A.Accent3Color() { RgbColorModelHex = new A.RgbColorModelHex() { Val = "9BBB59" } });
            colorScheme.Append(new A.Accent4Color() { RgbColorModelHex = new A.RgbColorModelHex() { Val = "8064A2" } });
            colorScheme.Append(new A.Accent5Color() { RgbColorModelHex = new A.RgbColorModelHex() { Val = "4BACC6" } });
            colorScheme.Append(new A.Accent6Color() { RgbColorModelHex = new A.RgbColorModelHex() { Val = "F366A7" } });
            colorScheme.Append(new A.Hyperlink() { RgbColorModelHex = new A.RgbColorModelHex() { Val = "0000FF" } });
            colorScheme.Append(new A.FollowedHyperlinkColor() { RgbColorModelHex = new A.RgbColorModelHex() { Val = "800080" } });
            themeElements.Append(colorScheme);

            // Font scheme
            var fontScheme = new A.FontScheme() { Name = "Office" };
            var majorFont = new A.MajorFont();
            majorFont.Append(new A.LatinFont() { Typeface = "Calibri" });
            majorFont.Append(new A.EastAsianFont() { Typeface = "" });
            majorFont.Append(new A.ComplexScriptFont() { Typeface = "" });
            fontScheme.Append(majorFont);

            var minorFont = new A.MinorFont();
            minorFont.Append(new A.LatinFont() { Typeface = "Calibri" });
            minorFont.Append(new A.EastAsianFont() { Typeface = "" });
            minorFont.Append(new A.ComplexScriptFont() { Typeface = "" });
            fontScheme.Append(minorFont);
            themeElements.Append(fontScheme);

            // Format scheme
            var formatScheme = new A.FormatScheme() { Name = "Office" };
            themeElements.Append(formatScheme);

            theme.Append(themeElements);
            themePart.Theme = theme;
        }
    }
}
