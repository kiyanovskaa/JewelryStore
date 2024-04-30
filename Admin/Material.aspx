<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Material.aspx.cs" Inherits="db_work.Admin.Material" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <script>
     window.onload = function () {
         var seconds = 5;
         setTimeout(function () {
             document.getElementById("<%=lblMsg.ClientID %>").style.display = "none";

         }, seconds * 1000);
     };
     </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div class="pcoded-inner-content pt-0">
     <div class="align-align-self-end">
         <asp:Label ID="lblMsg" runat="server" Visible="false"></asp:Label>
     </div>

     <div class="main-body">
         <div class="page-wrapper">

             <div class="page-body">
                 <div class="row">

                     <div class="col-sm-12">
                         <div class="card">
                             <div class="card-header">
                             </div>
                             <div class="card-block">
                                 <div class="row">
                                     <div class="col-sm-6 col-md-4 col-lg-4">

                                         <h4 class="sub-title">Material</h4>
                                         <div>
                                             <div class="form-group">
                                                 <label>Material Name</label>
                                                 <div>
                                                     <asp:TextBox ID="txtName" runat="server" CssClass="form-control"
                                                         placeholder="Enter category name" required></asp:TextBox>
                                                     <asp:HiddenField ID="hdnId" runat="server" Value="0" />
                                                 </div>
                                             </div>
                                          

                                             <div class="pb-5">
                                                 <asp:Button ID="btnAddOrUpdate" OnClick="btnAddOrUpdate_Click" runat="server" Text="Add" CssClass="btn btn-primary"
                                                     />
                                                 &nbsp;
                                                 <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-primary"
                                                     CausesValidation="false"  />
                                             </div>
                                           

                                         </div>
                                     </div>

                                     <div class="col-sm-6 col-md-8 col-lg-8 mobile-inputs">
                                         <h4 class="sub-title">Material List</h4>
                                         <div class="card-block table-border-style">
                                             <div class="table-responsive">
                                                 <asp:Repeater ID="rMaterial" runat="server" OnItemCommand="rMaterial_ItemCommand"
                                                      OnItemDataBound="rMaterial_ItemDataBound">

                                                     <HeaderTemplate>
                                                         <table class="table data-table-export table-hover nowrap">
                                                             <thead>


                                                                 <tr>
                                                                     <th class="table-plus">Name</th>
                                                                    
                                                                 </tr>
                                                             </thead>
                                                             <tbody>
                                                        
                                                     </HeaderTemplate>
                                                     <ItemTemplate>
                                                         <tr>
                                                             <td class="table-plus"><%# Eval("material_name") %> </td>
                                                            
                                                         
                                                             <td>
                                                                 <asp:LinkButton ID="lnkEdit" Text="Edit" runat="server" CssClass="badge badge-primary"
                                                                     CommandArgument='<%# Eval("material_id") %>' CommandName="edit" >
                                                                     <i class="ti-pencil"></i>
                                                                 </asp:LinkButton>

                                                                 <asp:LinkButton ID="lnkDelete" Text="Delete" runat="server" CommandName="delete"
                                                                     CssClass="badge bg-danger" CommandArgument='<%# Eval("material_id") %>'
                                                                     OnClientClick="return confirm('Do you want to delete this material?');" >
                                                                     <i class="ti-trash"></i>
                                                                 </asp:LinkButton>
                                                             </td>
                                                         </tr>
                                                     </ItemTemplate>
                                                     <FooterTemplate>
                                                         </tbody>
                                                         </table>
                                                     </FooterTemplate>
                                                 </asp:Repeater>
                                             </div>

                                         </div>
                                     </div>


                                 </div>
                             </div>
                         </div>
                     </div>
                 </div>
             </div>
         </div>
     </div>
 </div>
</asp:Content>
