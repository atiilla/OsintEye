   M S B A M L   `   `   `  ����C  ?GitRecon, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:GitReconGitRecon  5   	   7 clr-namespace:GitRecon.ResourcesGitRecon.Resources  5	   	       GitRecon.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:GitRecon     5   	   0res clr-namespace:GitRecon.Resources     5	   	      windowStyle#���   5   	   $	��366��5
   	   $	��563��6     � Title  5      `� 5   
    �� StringFormat$ 	{0} - {1}��6   .1�5      ��     GitRecon.Resources.Strings   AppTitle �� Source# Z 6   �� 5         Github# Z 6   5      .��5      � -   5         �Name$ MainGrid��5      $��	0,0,126,0q�6    � ContextMenu 5   
   �� 5      .@�5      m� -   5        $
��Dark��5      6)   m� 5      -   5        $��Light��5      6*   5      .+�5   
   �� -   5         ��Name$ MainApp��5      $��
0,0,-125,0q�6&     tabControlStyle#���  6:   .@�5      �� $��GitRecon��6     tabItemStyle#���  6(   .��5      �� -   5        $ MainTab��5      $��0,0,0,0q�6.   $��Stretch=�6?   $��Stretch=�6]   #���  6y   .@�5!      ��    TabMenu1#��Z 6   #���  6G   .��5"      �   	gridStyle#���  6   .+�5$      �� -   5        	 ��Name$	 
txUsername��5$   N     txtInputStyle#���  6&   $��10,5,0,0q�6b   $��3,2,3,2q�6t   $��Enter a username��6�   6�   �� 5&      -   5        
 ��Name$
 btnEmailQuery��5&   _     btnPrimaryStyle#���  6%   $��Query��6O   $��	197,5,0,0q�6v   $��3,2,3,2q�6�   6�   �� 5)        
lbApiToken#���  6(   $��	10,33,0,0q�6M   $��
API Token:��6`   �� 5*      -   5        $	 txToken��5*   N   #���  6&   $��	75,33,0,0q�6_   $	��200��6r   �� 5-      -	   5         ��Name$ isForksCheckBox��5-   h     chkBoxStyle#���  6'   $��Ignore all forks��6M   $��	10,58,0,0q�6�   �� 5.      -
   5         ��Name$ labelDg��5.   �     labelDg#���  6$   $+��%Hint: Double click on any row to copy��6F   $��	30,58,0,0q�6v   6�   &   System.Windows.Controls.DataGrid  50      -   5          Name$ ResultEmails��50   O    	 dataGridStyle#��� 	 6'      
IsReadOnly$
 True��6�   $��10,89,10,39q�6�      Columns 51   "   0  *System.Windows.Controls.DataGridTextColumn  52   &   ,  &System.Windows.Controls.DataGridColumn   Header$ Email��69   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6H   �� *Email+  53   &   $ Author��69    6I   �� *Author+55      �� 57      -   5         ��Name$ EmailsStatusTextBlock��57   U    
 tbStatusLabelStyle#��� 
 6(   $��
10,250,0,0q�6t   $��	Status...��6�   �� 58      -   5        $
 	btnExport��58   Z     	btnSaveAs#���  6%   $��Export��6I   $��0,0,16,7q�6m   $��6,3,6,3q�6   6�   59      5:      �� 5=         TabMenu2#��Z 6   #���  6G   .��5>      � #���  6   .+�5@      �� -   5        $	 txEmail��5@   N   #���  6&   $��10,5,0,0q�6_   $��3,2,3,2q�6q   $��Enter an email��6�   6�   �� 5B      -   5        $
 btnUsernameQuery��5B   _   #���  6%   $��Query��6O   $��	197,5,0,0q�6y   $��3,2,3,2q�6�   6�     5D      -   5        $ ResultUsernames��5D   '   $��10,60,10,39q�6@      AutoGenerateColumns$ False��6U    5E   "     5F   &   $ Username��69    6K   �� *
Username+5H      �� 5K      -   5        $ UsernameStatusTextBlock��5K   U   #��� 
 6(   $��
10,250,0,0q�6v   $��	Status...��6�   5L      5M      5O      5P      �� 5U      $��Social Account Finder��6   #���  65   .��5V      �  � RowDefinitions 5W      �� 5X      $
��Auto �6(   �� 5Y      $��* �6(   �� 5Z      $
��Auto �6(   .+�5]      ��  �� Orientation$ 
Horizontal=�6!   $��0��6:   $��Center=�6G   $��Center=�6d   .+�5^      �� -   5        $	 UsernameInput��5^   "   $��Enter a username...��69     txtSocialInput#���  6T   6}   �� 5`      -   5        $
 SearchButton��5`   !   $��Search��67     btnSocialQuery#���  6d   5a        5c      -   5        $ SocialMediaResults��5c      $ False��6;   $��
10,0,10,10q�6W   $
 True��6k   $��Stretch=�6}   $��1��6�    5d        5e      $ Profile URL��61      Width5  /System.Windows.Controls.DataGridLengthConverter$	 300 6e    6F   �� *
ProfileUrl+6e   5g      � 5i      $��5q�6   $��2��6&    � ColumnDefinitions 5j      �� 5k      $��* �6/   �� 5l      $
��Auto �6/   .+�5o      �� -   5        $ SocialAccountResult��5o   Q   #��� 
 6$   $��10,0,0,0q�6n   $��	Status...��6�   $
��Left=�6�   $��Center=�6�   $��0��6�   �� 5q      -   5        $
 btnSocialExport��5q   !   #���  6:   $��Export��6^   $	��100��6o   $��30��6{   $��5,0,10,0q�6�   $��Right=�6�   $��1��6�   5r      5s      5t      �� 5x      $��Subdomain Finder��6   #���  60   .��5y      �  5z      �� 5{      $
��Auto �6(   �� 5|      $��* �6(   �� 5}      $
��Auto �6(   .+�5�      �� $ 
Horizontal=�6!   $��0��6:   $��Center=�6G   $��Center=�6d   .+�5�      �� -   5        $	 DomainInput��5�   !   $��
domain.com��66   #���  6H   6q   �� 5�      -   5        $
 DomainSearchBtn��5�   !   $��Search��6:   #���  6j   5�        5�      -   5        $ SubDomainResults��5�      $ False��69   $��
10,0,10,10q�6U   $
 True��6i   $��Stretch=�6{   $��1��6�    5�        5�      $ Subdomain URL��61   $	 300 6f    6H   �� *	Subdomain+6f   5�      � 5�      $��5q�6   $��2��6&    5�      �� 5�      $��* �6/   �� 5�      $
��Auto �6/   .+�5�      �� -   5        $ Subdomainresult��5�   Q   #��� 
 6$   $��10,0,0,0q�6j   $��	Status...��6|   $
��Left=�6�   $��Center=�6�   $��0��6�   �� 5�      -   5        $
 btnSubdomainsExport��5�   !   #���  6>   $��Export��6b   $	��100��6s   $��30��6   $��5,0,10,0q�6�   $��Right=�6�   $��1��6�   5�      5�      5�      5�      5�      5�      