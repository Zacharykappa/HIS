﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:2.0.50727.4927
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// 此源代码是由 Microsoft.VSDesigner 2.0.50727.4927 版自动生成。
// 
#pragma warning disable 1591

namespace ts_SCM_HIS.WebReference {
    using System.Diagnostics;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="BasicHttpBinding_ISAPtoSCM", Namespace="http://tempuri.org/")]
    public partial class SAPtoSCM : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetPucharseFromSapOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetDrugFromHISOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public SAPtoSCM() {
            this.Url = global::ts_SCM_HIS.Properties.Settings.Default.ts_SCM_HIS_WebReference_SAPtoSCM;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event GetPucharseFromSapCompletedEventHandler GetPucharseFromSapCompleted;
        
        /// <remarks/>
        public event GetDrugFromHISCompletedEventHandler GetDrugFromHISCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ISAPtoSCM/GetPucharseFromSap", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void GetPucharseFromSap([System.Xml.Serialization.XmlArrayAttribute(IsNullable=true)] [System.Xml.Serialization.XmlArrayItemAttribute(Namespace="http://schemas.datacontract.org/2004/07/Ipedf.App.SAP_WCF")] Sap_PurchasePlan[] purcharseList, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string Flag, out bool GetPucharseFromSapResult, [System.Xml.Serialization.XmlIgnoreAttribute()] out bool GetPucharseFromSapResultSpecified) {
            object[] results = this.Invoke("GetPucharseFromSap", new object[] {
                        purcharseList,
                        Flag});
            GetPucharseFromSapResult = ((bool)(results[0]));
            GetPucharseFromSapResultSpecified = ((bool)(results[1]));
        }
        
        /// <remarks/>
        public void GetPucharseFromSapAsync(Sap_PurchasePlan[] purcharseList, string Flag) {
            this.GetPucharseFromSapAsync(purcharseList, Flag, null);
        }
        
        /// <remarks/>
        public void GetPucharseFromSapAsync(Sap_PurchasePlan[] purcharseList, string Flag, object userState) {
            if ((this.GetPucharseFromSapOperationCompleted == null)) {
                this.GetPucharseFromSapOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetPucharseFromSapOperationCompleted);
            }
            this.InvokeAsync("GetPucharseFromSap", new object[] {
                        purcharseList,
                        Flag}, this.GetPucharseFromSapOperationCompleted, userState);
        }
        
        private void OnGetPucharseFromSapOperationCompleted(object arg) {
            if ((this.GetPucharseFromSapCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetPucharseFromSapCompleted(this, new GetPucharseFromSapCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ISAPtoSCM/GetDrugFromHIS", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void GetDrugFromHIS([System.Xml.Serialization.XmlArrayAttribute(IsNullable=true)] [System.Xml.Serialization.XmlArrayItemAttribute(Namespace="http://schemas.datacontract.org/2004/07/Ipedf.App.SAP_WCF")] HIS_DRUG[] drugList, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string Flag, out bool GetDrugFromHISResult, [System.Xml.Serialization.XmlIgnoreAttribute()] out bool GetDrugFromHISResultSpecified) {
            object[] results = this.Invoke("GetDrugFromHIS", new object[] {
                        drugList,
                        Flag});
            GetDrugFromHISResult = ((bool)(results[0]));
            GetDrugFromHISResultSpecified = ((bool)(results[1]));
        }
        
        /// <remarks/>
        public void GetDrugFromHISAsync(HIS_DRUG[] drugList, string Flag) {
            this.GetDrugFromHISAsync(drugList, Flag, null);
        }
        
        /// <remarks/>
        public void GetDrugFromHISAsync(HIS_DRUG[] drugList, string Flag, object userState) {
            if ((this.GetDrugFromHISOperationCompleted == null)) {
                this.GetDrugFromHISOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetDrugFromHISOperationCompleted);
            }
            this.InvokeAsync("GetDrugFromHIS", new object[] {
                        drugList,
                        Flag}, this.GetDrugFromHISOperationCompleted, userState);
        }
        
        private void OnGetDrugFromHISOperationCompleted(object arg) {
            if ((this.GetDrugFromHISCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetDrugFromHISCompleted(this, new GetDrugFromHISCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.4927")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.datacontract.org/2004/07/Ipedf.App.SAP_WCF")]
    public partial class Sap_PurchasePlan {
        
        private string bEDATField;
        
        private string eBELNField;
        
        private string eBELPField;
        
        private string eINDTField;
        
        private string lGORTField;
        
        private string lIFNRField;
        
        private string mATNRField;
        
        private string mEINSField;
        
        private string mENGEField;
        
        private string mSEHTField;
        
        private string nAMEField;
        
        private string nETPRField;
        
        private string tXZ01Field;
        
        private string wERKSField;
        
        private string wERKSTField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string BEDAT {
            get {
                return this.bEDATField;
            }
            set {
                this.bEDATField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string EBELN {
            get {
                return this.eBELNField;
            }
            set {
                this.eBELNField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string EBELP {
            get {
                return this.eBELPField;
            }
            set {
                this.eBELPField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string EINDT {
            get {
                return this.eINDTField;
            }
            set {
                this.eINDTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string LGORT {
            get {
                return this.lGORTField;
            }
            set {
                this.lGORTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string LIFNR {
            get {
                return this.lIFNRField;
            }
            set {
                this.lIFNRField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string MATNR {
            get {
                return this.mATNRField;
            }
            set {
                this.mATNRField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string MEINS {
            get {
                return this.mEINSField;
            }
            set {
                this.mEINSField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string MENGE {
            get {
                return this.mENGEField;
            }
            set {
                this.mENGEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string MSEHT {
            get {
                return this.mSEHTField;
            }
            set {
                this.mSEHTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string NAME {
            get {
                return this.nAMEField;
            }
            set {
                this.nAMEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string NETPR {
            get {
                return this.nETPRField;
            }
            set {
                this.nETPRField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string TXZ01 {
            get {
                return this.tXZ01Field;
            }
            set {
                this.tXZ01Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string WERKS {
            get {
                return this.wERKSField;
            }
            set {
                this.wERKSField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string WERKST {
            get {
                return this.wERKSTField;
            }
            set {
                this.wERKSTField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.4927")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.datacontract.org/2004/07/Ipedf.App.SAP_WCF")]
    public partial class HIS_DRUG {
        
        private string cODEField;
        
        private string fACTORYField;
        
        private string mRJJField;
        
        private string nAMEField;
        
        private string sPECField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string CODE {
            get {
                return this.cODEField;
            }
            set {
                this.cODEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string FACTORY {
            get {
                return this.fACTORYField;
            }
            set {
                this.fACTORYField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string MRJJ {
            get {
                return this.mRJJField;
            }
            set {
                this.mRJJField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string NAME {
            get {
                return this.nAMEField;
            }
            set {
                this.nAMEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string SPEC {
            get {
                return this.sPECField;
            }
            set {
                this.sPECField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    public delegate void GetPucharseFromSapCompletedEventHandler(object sender, GetPucharseFromSapCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetPucharseFromSapCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetPucharseFromSapCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool GetPucharseFromSapResult {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
        
        /// <remarks/>
        public bool GetPucharseFromSapResultSpecified {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[1]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    public delegate void GetDrugFromHISCompletedEventHandler(object sender, GetDrugFromHISCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetDrugFromHISCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetDrugFromHISCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool GetDrugFromHISResult {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
        
        /// <remarks/>
        public bool GetDrugFromHISResultSpecified {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[1]));
            }
        }
    }
}

#pragma warning restore 1591