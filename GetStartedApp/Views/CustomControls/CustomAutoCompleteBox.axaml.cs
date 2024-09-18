using Avalonia;
using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GetStartedApp.Views.CustomControls
{
    public partial class CustomAutoCompleteBox : UserControl
    {
        public static readonly StyledProperty<string> SearchTextProperty =
            AvaloniaProperty.Register<CustomAutoCompleteBox, string>(nameof(SearchText));

        public string SearchText
        {
            get => GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        public static readonly StyledProperty<List<string>> ItemListBySearchTermProperty =
            AvaloniaProperty.Register<CustomAutoCompleteBox, List<string>>(nameof(ItemListBySearchTerm));

        public List<string> ItemListBySearchTerm
        {
            get => GetValue(ItemListBySearchTermProperty);
            set => SetValue(ItemListBySearchTermProperty, value);
        }

        public static readonly StyledProperty<bool> IsListBoxVisibleProperty =
            AvaloniaProperty.Register<CustomAutoCompleteBox, bool>(nameof(IsListBoxVisible));

        public bool IsListBoxVisible
        {
            get => GetValue(IsListBoxVisibleProperty);
            set => SetValue(IsListBoxVisibleProperty, value);
        }

        public static readonly StyledProperty<string> SelectedItemProperty =
            AvaloniaProperty.Register<CustomAutoCompleteBox, string>(nameof(SelectedItem));

        public string SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }


        public CustomAutoCompleteBox()
        {
            InitializeComponent();

            WhenTheUserMakeAserach_Update_And_DispalyTheNewsItemsFound();

            SetPopUpWidthToBeTheSameAsSearchBox();
        }

        // we do that to make pop up looks good becuase the widht of pup is dynamic depending on the item string wich makes it look bad
        private void SetPopUpWidthToBeTheSameAsSearchBox()
        {
            SearchTextBox.PropertyChanged += (s, e) =>
            {
                if (e.Property.Name == nameof(SearchTextBox.Bounds))
                {
                    PopupResults.Width = SearchTextBox.Bounds.Width;
                }
            };
        }

        private void WhenTheUserMakeAserach_Update_And_DispalyTheNewsItemsFound()
        {

           this.WhenAnyValue(x => x.SearchText).Subscribe(_ => IsListBoxVisible=true );

        }

    
        private void OnListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle item selection
            if (sender is ListBox listBox)
            {
                if (listBox.SelectedItem is string selectedItem)
                {
                    
                    SearchText = selectedItem;  // Update SearchText with the selected item
                  
                     IsListBoxVisible = false;   // Hide the list box

                }

                
                // we set selecteditem to null because when we set SeartchText = selectedItem in this case changing one of the both values will cause the event to get triggered when 
                // we change the searchText which cause problems because they are refrence types so they share the same address memory
                listBox.SelectedItem =null;
               
             
            }

        }
    }
}
