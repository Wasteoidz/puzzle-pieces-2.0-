using Microsoft.Maui.Layouts;

namespace PusleSpill;

public partial class GamePage : ContentPage
{
    private string selectedPuzzle;

    public GamePage(string puzzleName)
    {
        InitializeComponent();
        selectedPuzzle = puzzleName;
        SetupLayout();

        _ = LoadPuzzleAsync(); 
    }

    private void DisplayPieces(List<PuzzlePiece> pieces)
    {
        PuzzleBox.Children.Clear();

        if (pieces == null || pieces.Count == 0)
        {
            // Show error if no pieces
            PuzzleBox.Children.Add(new Label { Text = "No pieces loaded", TextColor = Colors.Red });
            return;
        }

        // Create a grid with 3 rows and 3 columns
        var pieceGrid = new Grid
        {
            RowDefinitions =
        {
            new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
            new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
            new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
        },
            ColumnDefinitions =
        {
            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
        }
        };

        foreach (var piece in pieces)
        {
            var pieceImage = new Image
            {
                Source = ImageSource.FromFile(piece.ImageFile),
                Aspect = Aspect.AspectFill, // Fill the available space
                BackgroundColor = Colors.LightGray
            };

            // Place each piece in its correct grid position
            Grid.SetRow(pieceImage, piece.Row);
            Grid.SetColumn(pieceImage, piece.Col);

            pieceGrid.Children.Add(pieceImage);
        }

        PuzzleBox.Children.Add(pieceGrid);
    }

    private async Task LoadPuzzleAsync()
    {
        try
        {
            string imageFile = selectedPuzzle switch
            {
                "Mad Cow!" => "mad_mech_cow.jpg",
                "Holy Cow!" => "holy_cow.jpg",
                "Battle Cow!" => "divergence.jpg",
                _ => "mad_mech_cow.jpg"
            };

            var pieces = await SplitImageAsync(imageFile, 3, 3);

            // Ensure UI updates on main thread
            MainThread.BeginInvokeOnMainThread(() => DisplayPieces(pieces));
        }
        catch (Exception ex)
        {
            // Temporary debug
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async Task<List<PuzzlePiece>> SplitImageAsync(string imageFile, int rows, int cols)
    {
        var pieces = new List<PuzzlePiece>();

        // For now, let's just verify the image files exist and can be loaded
        try
        {
            // Test loading one image to verify the path works
            var testImage = ImageSource.FromFile(imageFile);

            // If we get here, the image loads - continue creating pieces
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    pieces.Add(new PuzzlePiece
                    {
                        PieceId = row * cols + col,
                        ImageFile = imageFile,
                        CorrectPosition = new Point(col, row),
                        Row = row,
                        Col = col
                    });
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Image Error", $"Cannot load {imageFile}: {ex.Message}", "OK");
        }

        return pieces;
    }

    private void SetupLayout()
    {
        // Clear previous definitions
        MainLayout.RowDefinitions.Clear();
        MainLayout.ColumnDefinitions.Clear();

        bool isLandscape = Width > Height; // Simple orientation detection

        if (isLandscape)
        {
            // Landscape: Horizontal split
            MainLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            MainLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            Grid.SetColumn(PuzzleBoard, 0);
            Grid.SetColumn(PuzzleBox, 1);
            Grid.SetRow(PuzzleBoard, 0);
            Grid.SetRow(PuzzleBox, 0);
        }
        else
        {
            // Portrait: Vertical split  
            MainLayout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(3, GridUnitType.Star) });
            MainLayout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });

            Grid.SetRow(PuzzleBoard, 0);
            Grid.SetRow(PuzzleBox, 1);
            Grid.SetColumn(PuzzleBoard, 0);
            Grid.SetColumn(PuzzleBox, 0);
        }
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        SetupLayout(); // Re-layout when screen size changes
    }
}