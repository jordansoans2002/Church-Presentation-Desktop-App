using create_ppt_app.Model;
using create_ppt_app.MVVM;
using create_ppt_app.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace create_ppt_app.Command
{
    internal class AddSongCommand : CommandBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public AddSongCommand(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            //_mainWindowViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override void Execute(object? parameter)
        {
            if(parameter is SongDetailsViewModel song)
            {
                _mainWindowViewModel.SelectedSong = song;
                _mainWindowViewModel.SongDetails.Add(song);
            } else
            {
                Song s = new()
                {
                    Lang1 = _mainWindowViewModel.Song.Lang1,
                    Lang2 = _mainWindowViewModel.Song.Lang2
                };

                _mainWindowViewModel.SongDetails.Add(new SongDetailsViewModel(s));
                _mainWindowViewModel.SelectedSong = _mainWindowViewModel.SongDetails.Last();
            }
        }

        //public override bool CanExecute(object? parameter)
        //{
        //    song? selectedSong = parameter as song;
        //    if (selectedSong == null)
        //        return false;
        //    else if (
        //        (selectedSong.Text1 == null || selectedSong.Text1 == "") &&
        //        (selectedSong.Text2 == null || selectedSong.Text2 == "")
        //    )
        //        return false;

        //    // TODO may need to set limit on number of songs
        //    // based on the processing time and memory usage
        //    // if temp files are created more songs can be accomodated

        //    return true;
        //}

        //add code to re run CanExecute if values are updated
        //private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        //{
        //    if(e.PropertyName == nameof(MainWindowViewModel))
        //}
    }
}
