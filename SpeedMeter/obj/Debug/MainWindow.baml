   M S B A M L   `   `   `  ����E  ASpeedMeter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:SpeedMeter
SpeedMeter  5   	       SpeedMeter.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:SpeedMeter     5   	   -   5          � Title$  
SpeedMeter��5   	   $	��210��5	   	   $	��325��5
   	    � WindowStyle$
 None=�5   	    � 
ResizeMode$ NoResize=�5   	   
������  � WindowStartupLocation$ Manual=�5   	    � AllowsTransparency .  � 	Resources 5       �� 
TargetType   SmallButtonTop �� Property �� Value	 �� 
TargetType
 �� CornerRadius  SmallButtonBottom  RefreshButton �� Padding � ToolTip��%   &	        &	 �     &	 �    �� 5   
   " d�6   .�5      ��  �@�Cursor$
 Hand��6'   �� 5       � �� 5      �� 5      "	 d�6&   .R�5      �� $
 5��6!   ���#30303062   5      5      5      �� 5   
   " d�6   .�5      ��  �@�Cursor$
 Hand��6'   �� 5       � �� 5      �� 5       "	 d�6&   .R�5!      �� $
 5��6!   ���#30303062   5"      5$      5%      �� 5&   
   " ��6   .�5'      ��  �@�Cursor$
 Hand��6'   �� 5(       � �� 5)      �� 5*      "	 ��6&   .R�5+      �� $
 10��6!   ���#303030$ 3q�6H   $ Refresh IP List��6T   ._�5,      �� $y�Visible=�6$   $��/Icons/refresh-16.png��6:   5-      5.      50      51      52      .��53      �� $
 3��6   ���#0099FF$
��10pxq�55       J� Effect 56   
   3  -System.Windows.Media.Effects.DropShadowEffect  57         
BlurRadius$ 10Q�6      RenderingBias$ Quality=�6/      ShadowDepth$ 0Q�6G    � ContextMenu 59   
   �� 5:      $	��200��6   .@�5;      m� -   5        $��
mnuTopMost��5;   @    m� IsCheckable . $��Top most��6.   6R   m� 5<      -   5        $��mnuRunAtStartup��5<   F    . $��Run At Startup��6.   6   m� 5=      -   5        $��mnuExit��5=   =    .  $
��Exit��6/   6L   5>      ._�5@   
   �  � RowDefinitions 5A      �� 5B      $��25 �6    �� 5C      $��25 �6    �� 5D      $��* �6     � ColumnDefinitions 5F      �� 5G      $��30 �6#   �� 5H      $��70 �6#   �� 5I      $��40 �6#   �� 5J      $��25 �6#   �� 5K      $��30 �6#   �� 5L      $��70 �6#   �� 5M      $��40 �6#   .+�5Q      �� $��0��6   $��0��6%   $��Center=�65   $��Center=�6P   
�������$	��UL:��6�   �� 5R      -   5        $��	lblUpload��5R   �   $��0��6   $��1��6%   $��Center=�65   $��Center=�6P   
�������$��00.00��6�   6�   �� 5S      -   5        $��lblUploadUnit��5S   �   $��0��6   $��2��6%   $��Center=�65   $��Center=�6P   
�������$	��B/s��6�   6�   �� 5U      $��1��6   $��0��6%   $��Center=�65   $��Center=�6P   
�������$	��DL:��6�   �� 5V      -   5        $��lblDownload��5V   �   $��1��6   $��1��6%   $��Center=�65   $��Center=�6P   
�������$��00.00��6�   6�   �� 5W      -   5        $��lblDownloadUnit��5W   �   $��1��6   $��2��6%   $��Center=�65   $��Center=�6P   
�������$	��B/s��6�   6�   �� 5\      $��0��6   $��4��6%   $��Center=�65   $��Center=�6P   
�������$	��TU:��6�   �� 5]      -	   5        $��lblUploadTotal��5]   �   $��0��6   $��5��6%   $��Center=�65   $��Center=�6P   
�������$��00.00��6�   6�   �� 5^      -
   5        $��lblUploadTotalUnit��5^   �   $��0��6   $��6��6%   $��Center=�65   $��Center=�6P   
�������$	��B/s��6�   6�   �� 5`      $��1��6   $��4��6%   $��Center=�65   $��Center=�6P   
�������$	��TD:��6�   �� 5a      -   5        $��lblDownloadTotal��5a   �   $��1��6   $��5��6%   $��Center=�65   $��Center=�6P   
�������$��00.00��6�   6�   �� 5b      -   5        $��lblDownloadTotalUnit��5b   �   $��1��6   $��6��6%   $��Center=�65   $��Center=�6P   
�������$	��B/s��6�   6�   d� 5f      #��[  6   $��Center=�6C   $��0��6^   $��3��6k   $��10��6{   $��10��6�   $ Run Applications��6�   d� 5g      -   5        $��btnIPAddresses��5g       d� 	IsChecked$ FalseY�61   #��[ 6C   $��Center=�6n   $��1��6�   $��3��6�   $��10��6�   $��10��6�   $ Show IP Addresses��6�   6�   ]� 5k      -   5        $��	dockPanel��5k      $��3��6)   $��7��66   .+�5l      �� $	��Top=�6   $��0 1 0 0q�6.   ���#303030._�5m      ]� $
��auto��6$   .+�5n      �� -   5        $��btnRefreshIPAddresses��5n   <   $��Right=�6%   #��[ 6}   $��3q�6�   $��20��6�   $��20��6�   6�   �� 5o      6+   5p      5q      ]� 5r      $��Bottom=�6   $��20��64   .+�5s      �� $
��Left=�6    $��Center=�66   Finished in:6_   �� 5t      -   5        $��lblFinishedTime��5t   Q   $
��Left=�6    $��Center=�66   0s6l   �� 5u      -   5        $��
lblIPCount��5u   R   $��Right=�6    $��Center=�67   06g   �� 5v      $��Right=�6    $��Center=�67   Amount of IP's:6c   �� 5w      6#   5x      �� 5y      $
��Auto=�6   .��5z      &   System.Windows.Controls.DataGrid  -   5        $��grdIPAddresses��5z         AutoGenerateColumns .     
IsReadOnly . $	��288��5}         RowHeaderWidth$ 0Q�5~         SelectionMode$ Single=�5         Columns 5�      0  *System.Windows.Controls.DataGridTextColumn  5�      ,  &System.Windows.Controls.DataGridColumn   Header$
 Name��6J   1  +System.Windows.Controls.DataGridBoundColumn   Binding 61   �� *Name+6J     5�      $ 
IP Address��6O      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6c    61   �� *	IPAddress+6c   5�      5�      5�      5�      5�      5�      