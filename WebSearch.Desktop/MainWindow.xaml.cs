using Autofac;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using WebSearch.API.WebSearch;
using WebSearch.Desktop.Model;

namespace WebSearch.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IContainer IocContainer;
        private ISearchEngine Bing;
        private ISearchEngine Google;
        public MainWindow()
        {
            InitializeComponent();
            IocContainer = ConfigureIoc();
            Bing = IocContainer.ResolveNamed<ISearchEngine>("BING");
            Google = IocContainer.ResolveNamed<ISearchEngine>("GOOGLE");
        }

        private IContainer ConfigureIoc()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<BingSearchClient>().As<ISearchEngine>()
                   .Named<ISearchEngine>("BING").InstancePerDependency();

            builder.RegisterType<GoogleSearchClient>().As<ISearchEngine>()
                   .Named<ISearchEngine>("GOOGLE").InstancePerDependency();

            builder.RegisterType<CompositeSearchClient>()
                   .Named<ISearchEngine>("COMPOSITE")
                   .InstancePerDependency();

            var container = builder.Build();
            return container;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            GetSearchResults(SearchTerm.Text);
        }

        private void GetSearchResults(string searchTerm)
        {
            var cancelSource = new CancellationTokenSource();

            var bingTask = Bing.WebSearch(searchTerm);
            var googleTask = Google.WebSearch(searchTerm);
            

            Task.Factory.ContinueWhenAll(new[] { bingTask, googleTask }, (Task<IEnumerable<SearchResult>>[] tasks) =>
                {
                    var searchResults = new SearchResultsViewModel
                        {
                            BingSearchResults = new ObservableCollection<SearchResult>(tasks[0].Result),
                            GoogleSearchResults = new ObservableCollection<SearchResult>(tasks[1].Result)
                        };
                    DataContext = searchResults;

                }, cancelSource.Token, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
