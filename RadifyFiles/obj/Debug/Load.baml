   M S B A M L   `   `   `  ����B  >PGTAWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:PGTAWPFPGTAWPF  5          PGTAWPF.Load   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      'localclr-namespace:PGTAWPF     5        N� Title$
  Load��5	      .��5      � -   5        ���	#FF464646 � ColumnDefinitions 5   
   �� 5      $��60 �6   �� 5      $��* �6   �� 5      $��60 �6   �� 5      $��40 �6   �� 5      $��60 �6   �� 5      $��30 �6   �� 5      $	��180 �6   �� 5      $��30 �6   �� 5      $	��180 �6   �� 5      $��* �6   �� 5      $��60 �6    � RowDefinitions 5   
   �� 5      $��65 �6   �� 5      $
��Auto �6   �� 5      -   5         ��Name$ LabelRow��5   *   $
��Auto �6   6*   �� 5      $��30 �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5       $��* �6   �� 5!      $��40 �6   .+�5#   
   �� -   5         ��Name$ TitleLab��5#      $��1��6&   $��1��66   $��9��6C   ���	#FFB3BEA9$��Microsoft Sans Serif)�6n    �� TextAlignment$ Center=�6�   $
��Wrap=�6�   $��48(�6�   $
��0,15q�6�   $<��6Therere are no loaded files, plesase load one to start��6�   �� 5$   
   -   5        $ 	FilesList��5$      $��Holaa��6'   $��1��64   $��2��6D   $��9��6Q   ���	#FFB0BEA9$ Center=�6|   $��Microsoft Sans Serif)�6�   $��24(�6�   �� 5%   
   -   5         ��Name$ 
LoadSecond��5%   >   $��Upload File.png��6   $y�Visible=�6)   $��2��6R   $��3��6b   $��4��6v   $	��150��6�   6�   �� 5'   
   -   5        $ 	LoadFirst��5'   %   $��Loading.png��6   $��6��68   $��4��6H   $y�Visible=�6V   $��0,0,0,0q�6�   $	��100��6�   �� 5(   
   -   5        $ 
DeleteIcon��5(   $   $��
Delete.png��6   $��8��68   $��4��6H   $	��180��6U   $��0,0,0,10q�6b   $y�Visible=�6t   6�   �� 5)   
   -   5        $ 	ErrorIcon��5)   +   $��Error - copia.png��6   $��3��6>   $��5��6N   $��40��6[   $��0,0,0,0q�6g   $y�Visible=�6x   �� 5*   
   -	   5        $ 
ErrorLabel��5*      $��5��6)   $��4��66   $ Center=�6F   $��5��6]   $��Center=�6q   $
��Left=�6�   $
��10,0q�6�   $6��0Error loading the file. Please, try another one.��6�   ���	#FFC00000$��Microsoft Sans Serif)�6  $��22(�6&  5+      5,      