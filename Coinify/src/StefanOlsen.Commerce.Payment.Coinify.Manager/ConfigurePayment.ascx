<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfigurePayment.ascx.cs" Inherits="StefanOlsen.Commerce.Payment.Coinify.Manager.ConfigurePayment" %>
<div id="DataForm">
    <table cellpadding="0" cellspacing="2">
	    <tr>
		    <td class="FormLabelCell" colspan="2"><strong><asp:Literal runat="server" Text="Configure Coinify Account" /></strong></td>
	    </tr>
    </table>
    <br />
    <table class="DataForm">
	    <tr>
              <td class="FormLabelCell"><asp:Literal runat="server" Text="API Key" />:</td>
	          <td class="FormFieldCell">
		            <asp:TextBox Runat="server" ID="ApiKey" Width="300px"></asp:TextBox><br>
		            <asp:RequiredFieldValidator ControlToValidate="APIKey" Display="dynamic" Font-Name="verdana" Font-Size="9pt"
			                ErrorMessage="API key is required" runat="server" id="Requiredfieldvalidator1"></asp:RequiredFieldValidator>
	          </td>
        </tr>
         <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
        <tr>
            <td class="FormLabelCell"><asp:Literal runat="server" Text="API Secret" />:</td>
            <td class="FormFieldCell">
                <asp:TextBox Runat="server" ID="ApiSecret" Width="300px"></asp:TextBox><br>
                <asp:RequiredFieldValidator ControlToValidate="ApiSecret" Display="dynamic" Font-Name="verdana" Font-Size="9pt"
                                            ErrorMessage="API Secret is required" runat="server" id="Requiredfieldvalidator3"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
        <tr>
            <td class="FormLabelCell"><asp:Literal runat="server" Text="Hash Secret" />:</td>
            <td class="FormFieldCell">
                <asp:TextBox Runat="server" ID="HashSecret" Width="300px"></asp:TextBox><br>
                <asp:RequiredFieldValidator ControlToValidate="HashSecret" Display="dynamic" Font-Name="verdana" Font-Size="9pt"
                                            ErrorMessage="Hash Secret is required" runat="server" id="Requiredfieldvalidator6"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
        <tr>
            <td class="FormLabelCell"><asp:Literal runat="server" Text="Sandbox Mode?" />:</td>
            <td class="FormFieldCell">
                <asp:CheckBox Runat="server" ID="SandboxMode"/><br>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
        <tr>
            <td class="FormLabelCell"><asp:Literal runat="server" Text="Return URL" />:</td>
            <td class="FormFieldCell">
                <asp:TextBox Runat="server" ID="ReturnUrl" Width="300px"></asp:TextBox><br>
                <asp:RequiredFieldValidator ControlToValidate="ReturnUrl" Display="dynamic" Font-Name="verdana" Font-Size="9pt"
                                            ErrorMessage="Return URL is required" runat="server"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
        <tr>
            <td class="FormLabelCell"><asp:Literal runat="server" Text="Cancel URL" />:</td>
            <td class="FormFieldCell">
                <asp:TextBox Runat="server" ID="CancelUrl" Width="300px"></asp:TextBox><br>
                <asp:RequiredFieldValidator ControlToValidate="CancelUrl" Display="dynamic" Font-Name="verdana" Font-Size="9pt"
                                            ErrorMessage="Cancel URL is required" runat="server"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
        <tr>
            <td class="FormLabelCell"><asp:Literal runat="server" Text="Success URL" />:</td>
            <td class="FormFieldCell">
                <asp:TextBox Runat="server" ID="SuccessUrl" Width="300px"></asp:TextBox><br>
                <asp:RequiredFieldValidator ControlToValidate="SuccessUrl" Display="dynamic" Font-Name="verdana" Font-Size="9pt"
                                            ErrorMessage="Success URL is required" runat="server"></asp:RequiredFieldValidator>
            </td>
        </tr>
    </table>
</div>