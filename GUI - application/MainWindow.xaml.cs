using System;
using System.Windows;

namespace SimpleCalculator
{
    public partial class MainWindow : Window
    {
        private double _currentValue = 0;  // Gemmer den nuværende værdi af beregningen
        private string _lastOperation = string.Empty;  // Gemmer den sidste udførte operation for at kunne fortsætte beregningen
        private bool _isOperationPerformed = false;  // Indikerer om der er udført en operation, bruges til at styre input af tal

        public MainWindow()
        {
            InitializeComponent();
            txtDisplay.Text = string.Empty;  // Nulstil tekstfeltet ved opstart
        }

        // Håndtering af nummerknapper
        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;

            // Kontrollerer om knappen er null
            if (button == null) return;

            // Tjekker om en operation blev udført, eller hvis displayet viser en fejl eller er tomt
            if (_isOperationPerformed || txtDisplay.Text == "Fejl" || txtDisplay.Text == string.Empty)
            {
                txtDisplay.Text = button.Content.ToString();  // Sætter det valgte nummer i displayet
                _isOperationPerformed = false;  // Nulstil status for operation
            }
            else
            {
                txtDisplay.Text += button.Content.ToString();  // Tilføjer tallet til displayet
            }
        }

        // Håndtering af standard operationsknapper (+, -, *, /)
        private void OperationButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;

            // Kontrollerer at displayet ikke er tomt
            if (!string.IsNullOrEmpty(txtDisplay.Text))
            {
                // Udfør den tidligere operation, hvis en operation allerede er valgt
                if (!_isOperationPerformed && _lastOperation != string.Empty)
                {
                    EqualsButton_Click(sender, e);  // Kald EqualsButton for at beregne tidligere operation
                }

                // Hvis en operation allerede er udført, fjern den fra displayet
                if (_isOperationPerformed)
                {
                    txtDisplay.Text = txtDisplay.Text.TrimEnd(' ', _lastOperation[0]);  // Fjerner den sidste operation fra displayet
                }

                // Gemmer den nuværende værdi før operationen
                _currentValue = double.Parse(txtDisplay.Text);  // Parser eller analyserer den nuværende værdi til double
                _lastOperation = button.Content.ToString();  // Gemmer den valgte operation
                txtDisplay.Text = $"{_currentValue} {_lastOperation} ";  // Opdaterer displayet med operationen
                _isOperationPerformed = true;  // Markerer at en operation nu er udført
            }
        }

        // Håndtering af "=" knappen for at udføre beregningen
        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            // Tjekker om der er en sidste operation og om den aktuelle værdi kan parses
            if (!string.IsNullOrEmpty(_lastOperation) && double.TryParse(txtDisplay.Text.Split(' ')[^1], out double newValue))
            {
                try
                {
                    // Switch-case til at udføre de forskellige matematiske operationer
                    switch (_lastOperation)
                    {
                        case "+":
                            _currentValue += newValue;  // Addition
                            break;
                        case "-":
                            _currentValue -= newValue;  // Subtraktion
                            break;
                        case "*":
                            _currentValue *= newValue;  // Multiplikation
                            break;
                        case "/":
                            if (newValue != 0)
                                _currentValue /= newValue;  // Division, tjekker for division med nul
                            else
                                throw new DivideByZeroException("Division med nul er ikke tilladt.");  // Udløser fejl ved division med nul
                            break;
                        case "^":  // Potensberegning
                            _currentValue = Math.Pow(_currentValue, newValue);  // Beregner x^y
                            break;
                    }
                    txtDisplay.Text = _currentValue.ToString();  // Viser resultatet i displayet
                }
                catch (Exception ex)
                {
                    txtDisplay.Text = $"Fejl: {ex.Message}";  // Håndterer eventuelle fejl ved at vise en fejlmeddelelse
                }
                finally
                {
                    _lastOperation = string.Empty;  // Nulstil den sidste operation efter udførelse
                }
            }
        }

        // Håndtering af specialoperationer (sin, cos, tan, e^x)
        private void SpecialOperationButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;

            // Tjekker om værdien fra displayet kan parses til double
            if (double.TryParse(txtDisplay.Text, out double value))
            {
                try
                {
                    // Switch-case til at udføre de forskellige specialoperationer
                    switch (button.Content.ToString())
                    {
                        case "sin":
                            txtDisplay.Text = Math.Sin(value).ToString();  // Beregner sinus
                            break;
                        case "cos":
                            txtDisplay.Text = Math.Cos(value).ToString();  // Beregner cosinus
                            break;
                        case "tan":
                            txtDisplay.Text = Math.Tan(value).ToString();  // Beregner tangens
                            break;
                        case "e^x":
                            txtDisplay.Text = Math.Exp(value).ToString();  // Beregner e^x
                            break;
                    }
                    _currentValue = double.Parse(txtDisplay.Text);  // Gemmer resultatet
                    _lastOperation = string.Empty;  // Nulstil den sidste operation
                    _isOperationPerformed = true;  // Marker at en operation nu er udført
                }
                catch (Exception ex)
                {
                    txtDisplay.Text = $"Fejl: {ex.Message}";  // Håndterer fejl ved at vise en fejlmeddelelse
                }
            }
            else
            {
                txtDisplay.Text = "Fejl: Ugyldigt Input";  // Håndterer ugyldigt input
            }
        }

        // Håndtering af potens (x^y) knappen
        private void PowerButton_Click(object sender, RoutedEventArgs e)
        {
            // Kontrollerer at displayet ikke er tomt
            if (!string.IsNullOrEmpty(txtDisplay.Text))
            {
                // Udfør tidligere operation hvis relevant
                if (!_isOperationPerformed && _lastOperation != string.Empty)
                {
                    EqualsButton_Click(sender, e);  // Kald EqualsButton for at beregne tidligere operation
                }

                // Slet den sidste operation før at tilføje '^'
                if (_isOperationPerformed)
                {
                    txtDisplay.Text = txtDisplay.Text.TrimEnd(' ', _lastOperation[0]);  // Fjerner den tidligere operation fra displayet
                }

                // Gem den nuværende værdi før operationen
                _currentValue = double.Parse(txtDisplay.Text);  // Parser den nuværende værdi til double
                _lastOperation = "^";  // Marker operationen som potens
                txtDisplay.Text = $"{_currentValue} {_lastOperation} ";  // Opdaterer displayet med operationen
                _isOperationPerformed = true;  // Indiker at en operation er udført
            }
        }

        // Håndtering af clear-knappen
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            txtDisplay.Text = string.Empty;  // Nulstil displayet
            _currentValue = 0;  // Nulstil den nuværende værdi
            _lastOperation = string.Empty;  // Nulstil den sidste operation
            _isOperationPerformed = false;  // Nulstil status for operation
        }

        // Håndtering af backspace-knappen
        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            // Kontrollerer at displayet ikke er tomt og ikke viser en fejl
            if (!string.IsNullOrEmpty(txtDisplay.Text) && txtDisplay.Text != "Fejl")
            {
                txtDisplay.Text = txtDisplay.Text.Substring(0, txtDisplay.Text.Length - 1);  // Fjerner det sidste tegn fra displayet

                // Hvis displayet nu er tomt, sørg for at det forbliver tomt
                if (string.IsNullOrEmpty(txtDisplay.Text))
                {
                    txtDisplay.Text = string.Empty;  // Sætter displayet til tomt
                }
            }
        }
    }
}
