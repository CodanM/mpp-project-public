   M S B A M L   `   `   `  ����N  Jclient_wpf_remoting, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:client_wpf_remotingclient_wpf_remoting  5   	   $    client_wpf_remoting.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   3local!clr-namespace:client_wpf_remoting     5   	   -   5          � Title$  
MainWindow��5   	    � SizeToContent$ WidthAndHeight=�6   .��5	      � .+�5
   
   &   System.Windows.Controls.DataGrid  -   5          Name$ CompetitionsDataGrid��5
         AutoGenerateColumns .  $	��234��6e   $	��Top=�6r   $��
10,70,0,10q�6�   $
��Left=�6�   $	��339��6�   ��61   ��    Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ 30 6S   1  +System.Windows.Controls.DataGridBoundColumn   Binding 61   �� *CompetitionId+6S     5      $ Competition Type��6%   $	 150 6c    6?   �� *CompetitionType+6c     5      $ Age Category��6%   $ 85 6[    6;   �� *AgeCategory+6[     5      $ Count��6%   $ 45 6N    64   �� *Count+6N   5      �� 5   
   -   5         ��Name$ CompetitionTypeTextBox��5      $
��Left=�62   $��	10,41,0,0q�6M   	 �� TextWrapping$
	 Wrap=�6`   $	��Top=�6t   $	��120��6�   $��3,3,3,3q�6�   �� 5   
   $��Competition Type��6   $
��Left=�6+   $	��Top=�6F   $��	10,10,0,0q�6^   �� 5   
   -   5        $ AgeCategoryTextBox��5      $
��Left=�6.   $��
159,41,0,0q�6I   $
	 Wrap=�6]   $	��Top=�6q   $��99��6�   $��3,3,3,3q�6�   �� 5   
   $��Age Category��6   $
��Left=�6'   $��
159,10,0,0q�6B   $	��Top=�6V   �� 5   
   -   5        
 ��Name$
 SearchButton��5      $��Search��6'   $
��Left=�68   $��
292,41,0,0q�6S   $	��Top=�6g   $��24��6   $��57��6�   6�     5   
   -   5        $ ParticipantsDataGrid��5       .  $��363,70,10,0q�6e   $	��234��6z   $	��Top=�6�   $
��Left=�6�   $	��278��6�   ��61   ��  5        5      $ ID��6%   $ 30 6H    61   �� *Id+6H     5      $ 
First Name��6%   $	 100 6W    69   �� *	FirstName+6W     5      $ 	Last Name��6%   $	 100 6U    68   �� *
LastName+6U     5      $	 Age��6%   $ 35 6J    62   �� *Age+6J   5      �� 5    
   $��Participants��6   $
��Left=�6'   $��
363,39,0,0q�6B   $	��Top=�6V   �� 5!   
   -   5        $
 AddRegistrationButton��5!      $��Add Registration��60   $
��Left=�6K   $��
549,10,0,0q�6f   $��24��6z   $	��Top=�6�   6�   5#      5$      