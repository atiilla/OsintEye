   M S B A M L   `   `   `  ���� C  ?GitRecon, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:GitReconGitRecon  7 clr-namespace:GitRecon.ResourcesGitRecon.Resources      GitRecon.MainWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     (localclr-namespace:GitRecon     0res clr-namespace:GitRecon.Resources     $	��366��$	��563��  � Title  `�  �� StringFormat$ 	{0} - {1}��.1���     GitRecon.Resources.Strings   AppTitle �� Source# Z ��    Github# Z  � 	Resources  �� Source�� $ Resources/Styles.xaml?�%    .��� -    �Name$ MainGrid��$��	0,0,126,0q�.+��� -    ��Name$ MainApp��$��
0,0,-125,0q�.@��� $��GitRecon��.���� -   $ MainTab��$��0,0,0,0q�$��Stretch=�$��Stretch=�.@��� 	   TabMenu1#��Z	 .��� ���	#FFE5E5E5.+��� -   
 ��Name$
 
txUsername��   txtInputStyle#��[  $��10,5,0,0q�$��3,2,3,2q�$��Enter a username���� -    ��Name$ btnEmailQuery��  btnPrimaryStyle#��[ $��Query��$��	197,5,0,0q�$��3,2,3,2q���   
lbApiToken#��[ $��	10,33,0,0q�$��
API Token:���� -   $
 txToken��#��[  $��	75,33,0,0q�$	��200���� -    ��Name$ isForksCheckBox��  chkBoxStyle#��[ $��Ignore all forks��$��	10,58,0,0q��� -    ��Name$ labelDg��  labelDg#��[ $+��%Hint: Double click on any row to copy��$��	30,58,0,0q�&   System.Windows.Controls.DataGrid  -	     Name$ ResultEmails��  dataGridStyle#��[    
IsReadOnly$
 True��$��10,89,10,39q�   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ Email��1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *Email+  $ Author�� �� *Author+�� -
    ��Name$ EmailsStatusTextBlock��  tbStatusLabelStyle#��[ $��
10,250,0,0q�$��	Status...���� -   $ 	btnExport��  	btnSaveAs#��[ $��Export��$��0,0,16,7q�$��6,3,6,3q���    TabMenu2#��Z .��� ���	#FFE5E5E5.+��� -   $
 txEmail��#��[  $��10,5,0,0q�$��3,2,3,2q�$��Enter an email���� -   $ btnUsernameQuery��#��[ $��Query��$��	197,5,0,0q�$��3,2,3,2q�  -   $ ResultUsernames��$��10,60,10,39q�   AutoGenerateColumns$ False��   $ Username�� �� *
Username+�� -   $ UsernameStatusTextBlock��#��[ $��
10,250,0,0q�$��	Status...���� $��Social Account Finder��.���  � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+���  �� Orientation$ 
Horizontal=�$��0��$��Center=�$��Center=�.+��� -   $
 UsernameInput��$��Enter a username...��  txtSocialInput#��[ �� -   $ SearchButton��$��Search�� 	 btnSocialQuery#��[	   -   $ SocialMediaResults��$ False��$��
10,0,10,10q�$
 True��$��Stretch=�$��1��   $ Profile URL��   Width5  /System.Windows.Controls.DataGridLengthConverter$	 300  �� *
ProfileUrl+� $��5q�$��2�� � ColumnDefinitions �� $��* ��� $
��Auto �.+��� -   $ SocialAccountResult��#��[ $��10,0,0,0q�$��	Status...��$
��Left=�$��Center=�$��0���� -   $ btnSocialExport��#��[ $��Export��$	��100��$��30��$��5,0,10,0q�$��Right=�$��1���� $��Subdomain Finder��.���  �� $
��Auto ��� $��* ��� $
��Auto �.+��� $ 
Horizontal=�$��0��$��Center=�$��Center=�.+��� -   $
 DomainInput��$��
domain.com��#��[ �� -   $ DomainSearchBtn��$��Search��#��[	   -   $ SubDomainResults��$ False��$��
10,0,10,10q�$
 True��$��Stretch=�$��1��   $ Subdomain URL��$	 300  �� *	Subdomain+� $��5q�$��2�� �� $��* ��� $
��Auto �.+��� -   $ Subdomainresult��#��[ $��10,0,0,0q�$��	Status...��$
��Left=�$��Center=�$��0���� -   $ btnSubdomainsExport��#��[ $��Export��$	��100��$��30��$��5,0,10,0q�$��Right=�$��1��