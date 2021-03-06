﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.296
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.Silverlight.ServiceReference, version 4.0.50826.0
// 
namespace IP_Lab.MyService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="MyService.IPLabWebServiceSoap")]
    public interface IPLabWebServiceSoap {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/ExecuteCommand", ReplyAction="*")]
        System.IAsyncResult BeginExecuteCommand(IP_Lab.MyService.ExecuteCommandRequest request, System.AsyncCallback callback, object asyncState);
        
        IP_Lab.MyService.ExecuteCommandResponse EndExecuteCommand(System.IAsyncResult result);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ExecuteCommandRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="ExecuteCommand", Namespace="http://tempuri.org/", Order=0)]
        public IP_Lab.MyService.ExecuteCommandRequestBody Body;
        
        public ExecuteCommandRequest() {
        }
        
        public ExecuteCommandRequest(IP_Lab.MyService.ExecuteCommandRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class ExecuteCommandRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string command;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string data;
        
        public ExecuteCommandRequestBody() {
        }
        
        public ExecuteCommandRequestBody(string command, string data) {
            this.command = command;
            this.data = data;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ExecuteCommandResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="ExecuteCommandResponse", Namespace="http://tempuri.org/", Order=0)]
        public IP_Lab.MyService.ExecuteCommandResponseBody Body;
        
        public ExecuteCommandResponse() {
        }
        
        public ExecuteCommandResponse(IP_Lab.MyService.ExecuteCommandResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class ExecuteCommandResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string ExecuteCommandResult;
        
        public ExecuteCommandResponseBody() {
        }
        
        public ExecuteCommandResponseBody(string ExecuteCommandResult) {
            this.ExecuteCommandResult = ExecuteCommandResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPLabWebServiceSoapChannel : IP_Lab.MyService.IPLabWebServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ExecuteCommandCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public ExecuteCommandCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public string Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PLabWebServiceSoapClient : System.ServiceModel.ClientBase<IP_Lab.MyService.IPLabWebServiceSoap>, IP_Lab.MyService.IPLabWebServiceSoap {
        
        private BeginOperationDelegate onBeginExecuteCommandDelegate;
        
        private EndOperationDelegate onEndExecuteCommandDelegate;
        
        private System.Threading.SendOrPostCallback onExecuteCommandCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public PLabWebServiceSoapClient() {
        }
        
        public PLabWebServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public PLabWebServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PLabWebServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PLabWebServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Net.CookieContainer CookieContainer {
            get {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    return httpCookieContainerManager.CookieContainer;
                }
                else {
                    return null;
                }
            }
            set {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    httpCookieContainerManager.CookieContainer = value;
                }
                else {
                    throw new System.InvalidOperationException("无法设置 CookieContainer。请确保绑定包含 HttpCookieContainerBindingElement。");
                }
            }
        }
        
        public event System.EventHandler<ExecuteCommandCompletedEventArgs> ExecuteCommandCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult IP_Lab.MyService.IPLabWebServiceSoap.BeginExecuteCommand(IP_Lab.MyService.ExecuteCommandRequest request, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginExecuteCommand(request, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        private System.IAsyncResult BeginExecuteCommand(string command, string data, System.AsyncCallback callback, object asyncState) {
            IP_Lab.MyService.ExecuteCommandRequest inValue = new IP_Lab.MyService.ExecuteCommandRequest();
            inValue.Body = new IP_Lab.MyService.ExecuteCommandRequestBody();
            inValue.Body.command = command;
            inValue.Body.data = data;
            return ((IP_Lab.MyService.IPLabWebServiceSoap)(this)).BeginExecuteCommand(inValue, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        IP_Lab.MyService.ExecuteCommandResponse IP_Lab.MyService.IPLabWebServiceSoap.EndExecuteCommand(System.IAsyncResult result) {
            return base.Channel.EndExecuteCommand(result);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        private string EndExecuteCommand(System.IAsyncResult result) {
            IP_Lab.MyService.ExecuteCommandResponse retVal = ((IP_Lab.MyService.IPLabWebServiceSoap)(this)).EndExecuteCommand(result);
            return retVal.Body.ExecuteCommandResult;
        }
        
        private System.IAsyncResult OnBeginExecuteCommand(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string command = ((string)(inValues[0]));
            string data = ((string)(inValues[1]));
            return this.BeginExecuteCommand(command, data, callback, asyncState);
        }
        
        private object[] OnEndExecuteCommand(System.IAsyncResult result) {
            string retVal = this.EndExecuteCommand(result);
            return new object[] {
                    retVal};
        }
        
        private void OnExecuteCommandCompleted(object state) {
            if ((this.ExecuteCommandCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.ExecuteCommandCompleted(this, new ExecuteCommandCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void ExecuteCommandAsync(string command, string data) {
            this.ExecuteCommandAsync(command, data, null);
        }
        
        public void ExecuteCommandAsync(string command, string data, object userState) {
            if ((this.onBeginExecuteCommandDelegate == null)) {
                this.onBeginExecuteCommandDelegate = new BeginOperationDelegate(this.OnBeginExecuteCommand);
            }
            if ((this.onEndExecuteCommandDelegate == null)) {
                this.onEndExecuteCommandDelegate = new EndOperationDelegate(this.OnEndExecuteCommand);
            }
            if ((this.onExecuteCommandCompletedDelegate == null)) {
                this.onExecuteCommandCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnExecuteCommandCompleted);
            }
            base.InvokeAsync(this.onBeginExecuteCommandDelegate, new object[] {
                        command,
                        data}, this.onEndExecuteCommandDelegate, this.onExecuteCommandCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(callback, asyncState);
        }
        
        private object[] OnEndOpen(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndOpen(result);
            return null;
        }
        
        private void OnOpenCompleted(object state) {
            if ((this.OpenCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.OpenCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void OpenAsync() {
            this.OpenAsync(null);
        }
        
        public void OpenAsync(object userState) {
            if ((this.onBeginOpenDelegate == null)) {
                this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
            }
            if ((this.onEndOpenDelegate == null)) {
                this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
            }
            if ((this.onOpenCompletedDelegate == null)) {
                this.onOpenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnOpenCompleted);
            }
            base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginClose(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginClose(callback, asyncState);
        }
        
        private object[] OnEndClose(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndClose(result);
            return null;
        }
        
        private void OnCloseCompleted(object state) {
            if ((this.CloseCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void CloseAsync() {
            this.CloseAsync(null);
        }
        
        public void CloseAsync(object userState) {
            if ((this.onBeginCloseDelegate == null)) {
                this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
            }
            if ((this.onEndCloseDelegate == null)) {
                this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
            }
            if ((this.onCloseCompletedDelegate == null)) {
                this.onCloseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCloseCompleted);
            }
            base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
        }
        
        protected override IP_Lab.MyService.IPLabWebServiceSoap CreateChannel() {
            return new PLabWebServiceSoapClientChannel(this);
        }
        
        private class PLabWebServiceSoapClientChannel : ChannelBase<IP_Lab.MyService.IPLabWebServiceSoap>, IP_Lab.MyService.IPLabWebServiceSoap {
            
            public PLabWebServiceSoapClientChannel(System.ServiceModel.ClientBase<IP_Lab.MyService.IPLabWebServiceSoap> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginExecuteCommand(IP_Lab.MyService.ExecuteCommandRequest request, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = request;
                System.IAsyncResult _result = base.BeginInvoke("ExecuteCommand", _args, callback, asyncState);
                return _result;
            }
            
            public IP_Lab.MyService.ExecuteCommandResponse EndExecuteCommand(System.IAsyncResult result) {
                object[] _args = new object[0];
                IP_Lab.MyService.ExecuteCommandResponse _result = ((IP_Lab.MyService.ExecuteCommandResponse)(base.EndInvoke("ExecuteCommand", _args, result)));
                return _result;
            }
        }
    }
}
