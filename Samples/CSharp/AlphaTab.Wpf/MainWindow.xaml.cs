/*
 * This file is part of alphaTab.
 * Copyright (c) 2014, Daniel Kuschny and Contributors, All rights reserved.
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3.0 of the License, or at your option any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library.
 */
using System.Windows;
using AlphaTab.Wpf.Data;
using AlphaTab.Wpf.ViewModel;
using AlphaTab.Model;

namespace AlphaTab.Wpf.Gdi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();

            // ensure the UI is using en-us as culture 
            // we need this culture for correct WPF path string generation
            // in a German culture it could happen that we have a , as a decimal separator. 
            App.InitializeCultures();

            // create a our viewmodel for databinding
            viewModel = new MainViewModel(new DialogService(), new ErrorService());
            DataContext = viewModel;
            
            

        }

        public void SetHighlight(int index, bool isHighlighted)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var comp = GeneticMIDI.Representation.Composition.LoadFromMIDI(@"C:\Users\1gn1t0r\Documents\git\GeneticMIDI\GeneticMIDI\bin\Debug\test\harry.mid");
            var mel = comp.Tracks[0].GetMainSequence() as GeneticMIDI.Representation.MelodySequence;
            Score score = new Score();
            Track t = new Track();

            var pc = t.IsPercussion;

            MasterBar mb = new MasterBar();
            score.AddMasterBar(mb);
            mb.KeySignature = 2;  

            Bar b = new Bar();
            t.AddBar(b);
            score.AddTrack(t);

            Voice v = new Voice();
            b.AddVoice(v);

            int i = 0;

            int qn_per_bar = 4;

            int durs = 0;
            int avg_octave = mel.CalculateAverageOctave();
            int dist4 = 4 - avg_octave;
            foreach(var n in mel.Notes)
            {
                Beat be = new Beat();
                be.Index = i++;
                
                GeneticMIDI.Representation.Durations dur;
                int remainder;

                n.GetClosestLowerDurationAndRemainder(out dur, out remainder);

                int dots = n.GetNumberOfDots();

                durs += n.Duration;

        /*        if(durs >= qn_per_bar * (int)GeneticMIDI.Representation.Durations.qn)
                {
                    durs = 0;
                    b = new Bar();
                    t.AddBar(b);
                    v.Bar = b;
                    b.Finish();
                }*/

                switch (((GeneticMIDI.Representation.Durations)n.Duration))
                {
                    case GeneticMIDI.Representation.Durations.bn:
                        be.Duration = Model.Duration.Whole;
                        dots = 2;
                        break;
                    case GeneticMIDI.Representation.Durations.en:
                        be.Duration = Model.Duration.Eighth;
                        break;
                    case GeneticMIDI.Representation.Durations.hn:
                        be.Duration = Model.Duration.Half;
                        break;
                    case GeneticMIDI.Representation.Durations.qn:
                        be.Duration = Model.Duration.Quarter;
                        break;
                    case GeneticMIDI.Representation.Durations.sn:
                        be.Duration = Model.Duration.Sixteenth;
                        break;
                    case GeneticMIDI.Representation.Durations.tn:
                        be.Duration = Model.Duration.ThirtySecond;
                        break;
                    case GeneticMIDI.Representation.Durations.wn:
                        be.Duration = Model.Duration.Whole;
                        break;
                    default:
                        break;
                }
                be.Dots = dots;

                Note note = new Note();
                if (!n.IsRest())
                {
                    note.Tone = n.NotePitch;
                    note.Octave = n.Octave + dist4;
                    be.AddNote(note);
                    be.IsEmpty = false;

                    
                }

                if(n.IsRest() && n.Duration < 2)
                {

                }
                else
                    v.AddBeat(be);
                
                be.RefreshNotes();
                
                
               
            }
            
            

            v.Bar = b;

            v.Finish();

            b.Finish();
            
            t.Finish();

               
            score.Finish();
            
            


            viewModel.Score = score;
            viewModel.CurrentTrackIndex = 0;
            
            






        }
    }
}
