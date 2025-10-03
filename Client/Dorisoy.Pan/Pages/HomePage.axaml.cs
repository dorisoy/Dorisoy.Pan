using Dorisoy.PanClient.ViewModels;
namespace Dorisoy.PanClient.Pages;

public partial class HomePage : ReactiveUserControl<HomePageViewModel>
{
    private Grid _myMainGrid;
    private Border _box1, _box2, _box3, _box4, _box5, _box6, _box7, _box8, _box9, _box10, _box11;

    public HomePage()
    {
        this.InitializeComponent();
        this.SizeChanged += HomePage_SizeChanged;

        _myMainGrid = this.FindControl<Grid>("myMainGrid");
        _box1 = this.FindControl<Border>("box1");
        _box2 = this.FindControl<Border>("box2");
        _box3 = this.FindControl<Border>("box3");
        _box4 = this.FindControl<Border>("box4");
        _box5 = this.FindControl<Border>("box5");
        _box6 = this.FindControl<Border>("box6");
        _box7 = this.FindControl<Border>("box7");
        _box8 = this.FindControl<Border>("box8");
        _box9 = this.FindControl<Border>("box9");
        _box10 = this.FindControl<Border>("box10");
        _box11 = this.FindControl<Border>("box11");

        this.WhenActivated(disposable => { });
    }

    private void HomePage_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (this.Bounds.Width <= 1400)
        {
            _myMainGrid.ColumnDefinitions.Clear();

            _myMainGrid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
            _myMainGrid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));

            _myMainGrid.Margin = new Thickness(50);
            _myMainGrid.RowDefinitions.Clear();

            for (int row = 0; row < 8; row++)
            {
                if (row == 4)
                    _myMainGrid.RowDefinitions.Add(new RowDefinition(450, GridUnitType.Pixel));
                if (row == 7)
                    _myMainGrid.RowDefinitions.Add(new RowDefinition(170, GridUnitType.Pixel));
                else
                    _myMainGrid.RowDefinitions.Add(new RowDefinition(150, GridUnitType.Pixel));


            }

            Grid.SetRow(_box1, 0);
            Grid.SetColumn(_box1, 0);
            Grid.SetColumnSpan(_box1, 2);

            Grid.SetRow(_box2, 1);
            Grid.SetColumn(_box2, 0);
            Grid.SetColumnSpan(_box2, 2);

            Grid.SetRow(_box3, 2);
            Grid.SetColumn(_box3, 0);
            Grid.SetColumnSpan(_box3, 2);

            Grid.SetRow(_box4, 3);
            Grid.SetColumn(_box4, 0);
            Grid.SetColumnSpan(_box4, 2);

            Grid.SetRow(_box5, 4);
            Grid.SetColumn(_box5, 0);
            Grid.SetRowSpan(_box5, 1);

            Grid.SetRow(_box6, 4);
            Grid.SetColumn(_box6, 1);
            Grid.SetRowSpan(_box6, 1);

            Grid.SetRow(_box7, 5);
            Grid.SetColumn(_box7, 0);

            Grid.SetRow(_box8, 5);
            Grid.SetColumn(_box8, 1);

            Grid.SetRow(_box9, 6);
            Grid.SetColumn(_box9, 0);

            Grid.SetRow(_box10, 6);
            Grid.SetColumn(_box10, 1);

            Grid.SetRow(_box11, 7);
            Grid.SetColumn(_box11, 0);
            Grid.SetColumnSpan(_box11, 2);
        }
        else
        {
            _myMainGrid.Margin = new Thickness(50);

            _myMainGrid.RowDefinitions.Clear();
            for (int row = 0; row < 4; row++)
            {
                _myMainGrid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
            }

            _myMainGrid.ColumnDefinitions.Clear();
            for (int col = 0; col < 5; col++)
            {
                if (col == 0)
                    _myMainGrid.ColumnDefinitions.Add(new ColumnDefinition(480, GridUnitType.Pixel));
                else
                    _myMainGrid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
            }

            Grid.SetRow(_box1, 0);
            Grid.SetColumn(_box1, 0);
            Grid.SetColumnSpan(_box1, 1);

            Grid.SetRow(_box2, 1);
            Grid.SetColumn(_box2, 0);
            Grid.SetColumnSpan(_box2, 1);

            Grid.SetRow(_box3, 2);
            Grid.SetColumn(_box3, 0);
            Grid.SetColumnSpan(_box3, 1);

            Grid.SetRow(_box4, 3);
            Grid.SetColumn(_box4, 0);
            Grid.SetColumnSpan(_box4, 1);

            Grid.SetRow(_box5, 0);
            Grid.SetColumn(_box5, 1);
            Grid.SetRowSpan(_box5, 4);

            Grid.SetRow(_box6, 0);
            Grid.SetColumn(_box6, 2);
            Grid.SetRowSpan(_box6, 4);

            Grid.SetRow(_box7, 0);
            Grid.SetColumn(_box7, 3);

            Grid.SetRow(_box8, 0);
            Grid.SetColumn(_box8, 4);

            Grid.SetRow(_box9, 1);
            Grid.SetColumn(_box9, 3);

            Grid.SetRow(_box10, 1);
            Grid.SetColumn(_box10, 4);

            Grid.SetRow(_box11, 2);
            Grid.SetColumn(_box11, 3);
            Grid.SetColumnSpan(_box11, 2);
            Grid.SetRowSpan(_box11, 2);
        }

        _box1.Margin = new Thickness(10);
        _box2.Margin = new Thickness(10);
        _box3.Margin = new Thickness(10);
        _box4.Margin = new Thickness(10);
        _box5.Margin = new Thickness(10);
        _box6.Margin = new Thickness(10);
        _box7.Margin = new Thickness(10);
        _box8.Margin = new Thickness(10);
        _box9.Margin = new Thickness(10);
        _box10.Margin = new Thickness(10);
        _box11.Margin = new Thickness(10);
    }
}
