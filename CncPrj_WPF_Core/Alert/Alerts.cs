using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace CncPrj_WPF_Core.Alert
{
    internal class Alerts
    {
        Dictionary<string, List<ErrorAlert>> _errorAlerts;
        Dictionary<string, List<InfoAlert>> _informationAlerts;
        Dictionary<string, List<WarningAlert>> _warningAlerts;

        internal Alerts()
        {
            _errorAlerts = new Dictionary<string, List<ErrorAlert>>();
            _informationAlerts = new Dictionary<string, List<InfoAlert>>();
            _warningAlerts = new Dictionary<string, List<WarningAlert>>();
        }
        public void CreateAlert(AlertCategory alertCategory, String title, String message)
        {
            switch (alertCategory)
            {
                case AlertCategory.Error:
                    if (_errorAlerts.ContainsKey(title))
                    {
                        List<ErrorAlert> errorAlerts = _errorAlerts[title];
                        foreach (ErrorAlert errorAlert in errorAlerts)
                        {
                            if (errorAlert.GetErrorMessage().Equals(message))
                            {
                                errorAlert.CountUp();
                                break;
                            }
                        }
                    }
                    else
                    {
                        ErrorAlert errorAlert = new ErrorAlert(title, message);
                        errorAlert.Closed += RemoveErrorAlert;
                        List<ErrorAlert> errorAlerts = new List<ErrorAlert>();
                        errorAlerts.Add(errorAlert);
                        _errorAlerts.Add(title, errorAlerts);
                        errorAlert.ShowDialog();
                    }

                    break;
                case AlertCategory.Information:
                    if (_informationAlerts.ContainsKey(title))
                    {
                        List<InfoAlert> infoAlerts = _informationAlerts[title];
                        foreach (InfoAlert infoAlert in infoAlerts)
                        {
                            if (infoAlert.GetInformationMessage().Equals(message))
                            {
                                infoAlert.CountUp();
                                break;
                            }
                        }
                    }
                    else
                    {
                        InfoAlert infoAlert = new InfoAlert(title, message);
                        infoAlert.Closed += RemoveInformationAlert;
                        List<InfoAlert> infoAlerts = new List<InfoAlert>();
                        infoAlerts.Add(infoAlert);
                        _informationAlerts.Add(title, infoAlerts);
                        infoAlert.ShowDialog();
                    }
                    break;
                case AlertCategory.Warning:
                    if (_warningAlerts.ContainsKey(title))
                    {
                        List<WarningAlert> warningAlerts = _warningAlerts[title];
                        foreach (WarningAlert warningAlert in warningAlerts)
                        {
                            if (warningAlert.GetWarningMessage().Equals(message))
                            {
                                warningAlert.CountUp();
                                break;
                            }
                        }
                    }
                    else
                    {
                        WarningAlert warningAlert = new WarningAlert(title, message);
                        warningAlert.Closed += RemoveWariningAlert;
                        List<WarningAlert> warningAlerts = new List<WarningAlert>();
                        warningAlerts.Add(warningAlert);
                        _warningAlerts.Add(title, warningAlerts);
                        warningAlert.ShowDialog();
                    }
                    break;
            }
        }
        public int CountErrorAlerts()
        {
            return _errorAlerts.Count;
        }
        public int CountInformationAlerts()
        {
            return _informationAlerts.Count;
        }
        public int CountWarningAlerts()
        {
            return _warningAlerts.Count;
        }
        void RemoveErrorAlert(object sender, EventArgs eventArgs)
        {
            ErrorAlert errorAlert = (ErrorAlert)sender;

            if (_errorAlerts.ContainsKey(errorAlert.GetErrorMessageTitle()))
            {
                List<ErrorAlert> errorAlerts = _errorAlerts[errorAlert.GetErrorMessageTitle()];
                foreach (ErrorAlert item in errorAlerts)
                {
                    if (item.GetErrorMessage().Equals(errorAlert.GetErrorMessage()))
                    {
                        errorAlerts.Remove(item);
                        break;
                    }
                }
                if (errorAlerts.Count.Equals(0))
                {
                    _errorAlerts.Remove(errorAlert.GetErrorMessageTitle());
                }
            }
        }
        void RemoveInformationAlert(object sender, EventArgs eventArgs)
        {
            InfoAlert infoAlert = (InfoAlert)sender;

            if (_informationAlerts.ContainsKey(infoAlert.GetInformationMessageTitle()))
            {
                List<InfoAlert> infoAlerts = _informationAlerts[infoAlert.GetInformationMessageTitle()];
                foreach (InfoAlert item in infoAlerts)
                {
                    if (item.GetInformationMessage().Equals(infoAlert.GetInformationMessage()))
                    {
                        infoAlerts.Remove(item);
                        break;
                    }
                }
                if (infoAlerts.Count.Equals(0))
                {
                    _informationAlerts.Remove(infoAlert.GetInformationMessageTitle());
                }
            }
        }
        void RemoveWariningAlert(object sender, EventArgs eventArgs)
        {
            WarningAlert warningAlert = (WarningAlert)sender;
            if (_warningAlerts.ContainsKey(warningAlert.GetWarningMessageTitle()))
            {
                List<WarningAlert> warningAlerts = _warningAlerts[warningAlert.GetWarningMessageTitle()];
                foreach (WarningAlert item in warningAlerts)
                {
                    if (item.GetWarningMessage().Equals(warningAlert.GetWarningMessage()))
                    {
                        warningAlerts.Remove(item);
                        break;
                    }
                }
                if (warningAlerts.Count.Equals(0))
                {
                    _warningAlerts.Remove(warningAlert.GetWarningMessageTitle());
                }
            }
        }
    }
    public enum AlertCategory
    {
        Error,
        Information,
        Warning,
    }
}
