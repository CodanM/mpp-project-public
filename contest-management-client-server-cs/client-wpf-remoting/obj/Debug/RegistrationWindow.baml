   M S B A M L   `   `   `  ����N  Jclient_wpf_remoting, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:client_wpf_remotingclient_wpf_remoting  5   	   ,    &client_wpf_remoting.RegistrationWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   3local!clr-namespace:client_wpf_remoting     5   	     � Title$  RegistrationWindow��5   	    � SizeToContent$ WidthAndHeight=�6$   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $��1* �6   $��70��6*   �� 5      $
��1.5* �6   �� 5      $��1* �6   $��70��6*   .+�5   
   �� -   5         ��Name$ ExistsCheckBox��5      $��Existing Participant��6+   $��Center=�6J   $��0,20,0,0q�6g   $	��Top=�6y   $��1��6�   6�   �� 5   
   $��
First Name��6   $
��Left=�6%   $��	10,40,0,0q�6@   $	��Top=�6S   $��1��6k   �� 5   
   -   5         ��Name$ FirstNameTextBox��5      $��1��6,   $��Center=�6<   $��0,71,0,0q�6Y    �� TextWrapping$
 Wrap=�6k   $	��Top=�6   $	��118��6�   $��3,3,3,3q�6�   �� 5   
   $��	Last Name��6   $��1��6$   $
��Left=�64   $��
10,100,0,0q�6O   $	��Top=�6c   �� 5   
   -   5        $ LastNameTextBox��5      $��1��6+   $��Center=�6;   $��	0,131,0,0q�6X   $
 Wrap=�6k   $	��Top=�6   $	��117��6�   $��3,3,3,3q�6�   �� 5   
   $��ID��6   $��1��6   $
��Left=�6-   $��
10,160,0,0q�6H   $	��Top=�6\   �� 5   
   -   5         ��Name$ 
IdComboBox��5      $��1��6'   $��Center=�67   $��	0,191,0,0q�6T   $	��Top=�6g   $	��117��6   {�.  6�   �� 5   
   $	��Age��6   $��1��6   $
��Left=�6.   $��
10,218,0,0q�6I   $	��Top=�6]   �� 5   
   -   5         ��Name$ DecrementAgeButton��5      $��-��6-   $��1��69   $
��Left=�6I   $��
12,249,0,0q�6d   $	��Top=�6x   $��24��6�   $��24��6�   6�   �� 5   
   -   5        $ IncrementAgeButton��5      $��+��6-   $��1��69   $
��Left=�6I   $��
84,249,0,0q�6d   $	��Top=�6x   $��24��6�   $��24��6�   6�   �� 5   
   $��Competition Type��6   $��1��6+   $
��Left=�6;   $��
10,278,0,0q�6V   $	��Top=�6j   �� 5   
   -   5        $ CompetitionTypeComboBox��5      $��1��64   $��Center=�6D   $��	0,309,0,0q�6a   $	��Top=�6t   $	��117��6�    �� 
IsEditable . 6�   �� 5   
   $��Age Category��6   $��1��6'   $
��Left=�67   $��
10,336,0,0q�6R   $	��Top=�6f   �� 5   
   -   5        $ AgeCategoryComboBox��5      $��1��60   $��Center=�6@   $��	0,367,0,0q�6]   $	��Top=�6p   $	��117��6�   �� 5   
   -	   5        $ 	AddButton��5      $	��Add��6$   $��1��62   $
��Left=�6B   $��43,406,0,20q�6]   $	��Top=�6r   $��3,3,3,3q�6�   6�   �� 5   
   -
   5        	 ��Name$	 AgeTextBlock��5      $��1��6*   $��Center=�6:   $��	0,249,0,0q�6W   $
��Wrap=�6j   $	��Top=�6~   $��24��6�   $��38��6�   
 �� TextAlignment$
 Center=�6�    �� Padding$ 3,3,3,3q�6�   $��6��6�   5      5       