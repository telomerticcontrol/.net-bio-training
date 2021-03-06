﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Lab05.Infrastructure;
using Bio;
using Bio.IO;
using Microsoft.Win32;

namespace Lab06.ViewModels
{
    /// <summary>
    /// Primary ViewModel that manages the application state and logic.
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private SequenceViewModel _selectedSequence;
        private string _selectedSequenceFragment;

        /// <summary>
        /// Loaded sequences
        /// </summary>
        public IList<SequenceViewModel> LoadedSequences { get; private set; }

        /// <summary>
        /// Returns the selected sequence
        /// </summary>
        public SequenceViewModel SelectedSequence
        {
            get { return _selectedSequence; }
            set
            {
                _selectedSequence = value;
                OnPropertyChanged("SelectedSequence");
            }
        }

        /// <summary>
        /// The fragment selected from the sequence
        /// </summary>
        public string SelectedSequenceFragment
        {
            get { return _selectedSequenceFragment; }
            set { _selectedSequenceFragment = value; OnPropertyChanged("SelectedSequenceFragment"); }
        }

        /// <summary>
        /// Command used to import
        /// </summary>
        public ICommand ImportFile { get; private set; }

        /// <summary>
        /// Run BLAST
        /// </summary>
        public ICommand RunBlast { get; private set; }

        /// <summary>
        /// Used to cancel a pending request
        /// </summary>
        public ICommand CancelBlast { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MainViewModel()
        {
            LoadedSequences = new ObservableCollection<SequenceViewModel>();
            ImportFile = new DelegatingCommand(OnLoadFile);
            RunBlast = new DelegatingCommand(OnBlastSequence, () => SelectedSequence != null);
            CancelBlast = new DelegatingCommand(OnCancelBlast);
        }

        /// <summary>
        /// Used to display a message on the screen when an exception occurs.
        /// </summary>
        /// <param name="title">Title of the message</param>
        /// <param name="message">Message to display</param>
        /// <param name="exception">Exception that has occurred</param>
        private static void ShowError(string title, string message, Exception exception)
        {
            string errorMessage = (exception != null) ? exception.Message : string.Empty;
            MessageBox.Show(string.Format("{0}{1}{2}", message, string.IsNullOrEmpty(errorMessage) ? "" : ": ", errorMessage), title);
        }

        /// <summary>
        /// This method loads new sequences from a file.
        /// </summary>
        private void OnLoadFile()
        {
            string filterString = "All Supported Formats|" + string.Join(";", SequenceParsers.All.Select(parser => parser.SupportedFileTypes.Replace(',', ';').Replace(".", "*."))) + "|" +
                string.Join("|", SequenceParsers.All.Select(parser => string.Format("{0}|{1}", parser.Name, parser.SupportedFileTypes.Replace(',', ';').Replace(".", "*."))));

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = filterString
            };

            // Prompt the user for the filename
            if (openFileDialog.ShowDialog() == true)
            {
                // See if we can auto-locate the parser
                ISequenceParser parser = SequenceParsers.FindParserByFileName(openFileDialog.FileName);
                if (parser == null)
                {
                    // Use the extension
                    string fileExtension = Path.GetExtension(openFileDialog.FileName);
                    parser = SequenceParsers.All.FirstOrDefault(sp => sp.SupportedFileTypes.Contains(fileExtension));
                    if (parser != null)
                    {
                        parser.Open(openFileDialog.FileName);
                    }
                }

                // Cannot parse this file.
                if (parser == null)
                {
                    MessageBox.Show(string.Format("Cannot locate a sequence parser for {0}", openFileDialog.FileName), "Cannot Parse File");
                    return;
                }

                // Parse the file - open it read-only as we will not be writing the sequences back out.
                try
                {
                    foreach (var sequence in parser.Parse())
                        LoadedSequences.Add(new SequenceViewModel(sequence));

                }
                catch (Exception ex)
                {
                    ShowError("Cannot Parse File", "Failed to open " + openFileDialog.FileName, ex);
                }
            }
        }

        /// <summary>
        /// This method is called when the user wants to run BLAST on a sequence
        /// </summary>
        private void OnBlastSequence()
        {
            // Get the full sequence, or the selected fragment
            ISequence sequence = !string.IsNullOrEmpty(SelectedSequenceFragment)
                ? new Sequence(SelectedSequence.RawSequence.Alphabet, SelectedSequenceFragment)
                : SelectedSequence.RawSequence;

            // TODO: implement BLAST call
        }

        /// <summary>
        /// Used to cancel a pending request
        /// </summary>
        private void OnCancelBlast()
        {
            // TODO: implement cancelation
        }
    }
}
