// ==========================================================================
// $Workfile:   BILLING.C  $
// $Revision:   1.19  $
//
// HISTORY:
// Author				Date		Description
// --------------------------------------------------------------------------
//
//
//
// DESCRIPTION
// ==========================================================================
//	Copyright 1996 TeCom Inc. All rights reserved
// ==========================================================================

#include <stdio.h>
#include <stdlib.h>
#include <dos.h>
#include <time.h>
#include <string.h>
#include <math.h>
#include <bios.h>
#include <conio.h>
#include <sys\types.h>
#include <sys\stat.h>
#include <direct.h>
#include <ctype.h>

#include "2box.h"
#include "cebus.h"
#include "inside.h"
#include "vga.h"
#include "scrnobjt.h"
#include "textbox.h"
#include "pshbtn.h"
#include "lwindow.h"
#include "uumenu.h"
#include "calendar.h"
#include "fnsmsg.h"
#include "device.h"
#include "inhist.h"
//#include "dsdmenu.h"
#include "whmenu.h"
#include "inutil.h"
#include "inio.h"
#include "insubs.h"
#include "rate.h"
#include "graphs.h"
#include "msgbox.h"
#include "ismenu.h"
#include "calendar.h"
#include "device.h"
#include "listbox.h"
#include "tmentry.h"
#include "spinner.h"
#include "mouse.h"
#include "scrolbar.h"

// mark
typedef struct
{
	int	begin_day;
	int	begin_month;
	int	begin_year;

	int	end_day;
	int	end_month;
	int end_year;
}DATE_ENTRY_T;

extern USHORT		days_tab[2][13];

extern void		FirstHistory(void);

void			BillingMenu(void);

static void		DisplayDateEntry(DATE_ENTRY_T *de, int top, int left, UCHAR color);
static void		AdvancedBillView(void);
static void		AddDate(void);
static void		ChangeDate(void);
static void		RemoveDateEntry(void);
static void		FixDates(DATE_ENTRY_T *de);
static void		AdvancedShow(void);
static void		AdvancedClick(void);
static int		Clean(UCHAR *line);
static void		Read(UCHAR *linein, FILE *prgfile);
static int		Process(UCHAR *line);
static int		GetKeyWord(UCHAR *line, UCHAR *bookmark);
static void		Print(UCHAR *str);
static int		ProcessVariable(UCHAR *bookmark);
static double	GetValue(UCHAR *name);
static double	GetTableValue(UCHAR *cp1, UCHAR *cp2);
static double	Evaluate(UCHAR *cp);
static void		SaveDates(void);
static void		OpenDates(void);
static void		RefreshClick(void);
static void		ViewRateClick(void);
static void 	CalcLoadCtrlCreditPenalty	(
												int month,
												int day,
												int year,
												DAYS_RATES *days_rates,
												double	*lc_kwh_saved,
												double	*lc_credit$,
												double	*lc_kwh_used,
												double	*lc_penalty$
											);


#define P_SUCCESS 0
#define P_FAILURE 1
#define TRUE 1
#define FALSE 0

#define MAX_KEYWORDS 8
#define MAX_OPERATORS 7
#define MAX_VARIABLES 250
#define MAX_VARIABLE_NAME 20
#define MAX_ARRAYS 5
#define MAX_LINES 27 // first six lines are used by system.
#define MAX_COLUMNS 78
static int my_hokey_flag;

static int		current_line;
static UCHAR	process_if[100];
static UCHAR	if_level;


static int acd;	// add or change date flaf for advanced functions

void	(*BM_addr)() = BillingMenu;

DATE_ENTRY_T *datesublist[37];
static LISTBOX_T list_box	=
{		// BEGIN SCREEN OBJECT DEFINITION
	108, 200, 220, 230,    // position
	FALSE,					// focus
	LIST_BOX,			// type
	TRUE,				// visible
	ListBoxMouseDown,   // OnMouseDown()
	ListBoxMouseUp,		// OnMouseUp()
	ListBoxClick,		// OnClick()
	ListBoxKeyPress,	// OnKeyPress()
	PutListBox,			// DisplayObject()
	// END SCREEN OBJECT DEFINITION
	0,					// item count
	0,					// item selected
	0,					// top_item 
	0,					// display_count
	datesublist,		// sub_list
	DisplayDateEntry	// display function pointer
};


static void sbOnClick(int direction, int how_far, int thumb_pos);

static SCROLL_BAR_T advancedsb =
{	// BEGIN SCREEN OBJECT DEFINITION
	108, 420, 17, 230,	// position
	FALSE,				// focus
	SCROLL_BARV,		// type
	TRUE,				// visible
	ScrollBarOnMouseDown,// OnMouseDown()
	ScrollBarOnMouseUp,				// OnMouseUp()
	ScrollBarOnClick,	// OnClick()
	NULL,				// OnKeyPress()
	DisplayScrollBar,	// DisplayObject()
	// END SCREEN OBJECT DEFINITION
	0,		// min
	15, 	// max
	1,		// small change
	5,		// large change
	0,		// thumb possition
	SB_VERTICAL,	// type
	sbOnClick	// on scroll function
};

static void sbOnClick(int direction, int how_far, int thumb_pos)
{
	switch(direction)
	{
		case SCROLL_UP:
			switch(how_far)
			{
				case SCROLL_LARGE:
					PageUp(&list_box);
					break;
				case SCROLL_SMALL:
					UpArrow(&list_box);
					break;
			}
			break;
		case SCROLL_DOWN:
			switch(how_far)
			{
				case SCROLL_LARGE:
					PageDown(&list_box);
					break;
				case SCROLL_SMALL:
					DownArrow(&list_box);
					break;
			}
			break;
	}
	ListBoxScroll(&list_box);
	//PutListBox(&list_boxd);
	if (list_box.OnChange != NULL)
	{
		(*list_box.OnChange)();
	}
}

typedef struct
{
	UCHAR name[26];	// allows 25 char name
	UCHAR type;		//0 = single element, 1 = array memeber
	UCHAR index;	//index from 0
	double value;
}VARIABLE;

double	arrays[256][MAX_ARRAYS];
int		array_count;

VARIABLE variable[MAX_VARIABLES];
int var_count;

UCHAR *month_name[12] =
{ 
	"January",
	"February",
	"March",
	"April",
	"May",
	"June",
	"July",
	"August",
	"September",
	"October",
	"November",
	"December"
};

UCHAR *keywords[MAX_KEYWORDS] =
{
	"NEWLINE",	// 0
	"GOTO",		// 1
	"PRINT",	// 2
	"LINE",		// 3
	"ENDIF",	// 4
	"IF",		// 5
	"ELSE",		// 6
	"%"
};

//operators are stored in order of precidence
UCHAR *operators[MAX_OPERATORS] =
{
	"(",
	")",
	"*",
	"/",
	"+",
	"-",
	"%"
};

int BILLING_STATEMENT_MAX;

extern BILLING_CONSTANTS	billing_constants;
extern struct tm *tod;

#define MAX_DATES 100
DATE_ENTRY_T date_list[MAX_DATES];
int date_count;

DATE_ENTRY_T bill_date;
static void	BillingStatement(void);
static void	CalculateBill(void);
static void billBackBtnClk(void);
static void billNextBtnClk(void);
static void billBackYrBtnClk(void);
static void billNextYrBtnClk(void);
static void SpinOnChange(void);

SPINNER_T spin =
{
	435, 5, 1, 1,    // position
	FALSE,				//focus
	TIME_ENTRY,			//type
	TRUE,				// visible
	SpinnerMouseDown,   //OnMouseDown()
	SpinnerMouseUp,		//OnMouseUp()
	SpinnerClick,		// OnClick()
	SpinnerKeyPress,	// OnKeyPress()
	DisplaySpinner,		// DisplayObject()
	1,					//value
	31,					//max
	1,					//min
	1,					//increment
	SpinOnChange
};

static void SpinOnChange(void)
{
	FILE *f;
	
	if ((f=fopen("bill.dat", "wb")) == NULL)
	{
		msgbox("Disk may be full.|Exit LaneView and check|your hard disk.", "Disk Error", MB_OK);
		return;
	}
	fwrite( &spin.value, sizeof(int), 1, f );
	fclose(f);
	bill_date.begin_day = spin.value;
	bill_date.end_day = spin.value;
	//BillingStatement();
}

PUSH_BUTTON_T billBtnList[8] =
{
	419, 70, 89, 33,    // position
	FALSE,				//focus
	PUSH_BUTTON,	//type
	TRUE,				// visible
	BtnMouseDown,   //OnMouseDown()
	BtnMouseUp,		//OnMouseUp()
	BtnClick,		// OnClick()
	BtnKeyPress,	// OnKeyPress()
	PutButton,		// DisplayObject()
	FALSE,			//default button
	"Refresh",		//text
	push,			//type
	up,				//state
	NULL,           //icon
	RefreshClick,	//function
	74,

	419, 165, 89, 33,    // position
	FALSE,				//focus
	PUSH_BUTTON,	//type
	TRUE,				// visible
	BtnMouseDown,   //OnMouseDown()
	BtnMouseUp,		//OnMouseUp()
	BtnClick,		// OnClick()
	BtnKeyPress,	// OnKeyPress()
	PutButton,		// DisplayObject()
	FALSE,				//default button
	"Advanced",	//text
	push,			//type
	up,				//state
	NULL,           //icon
	AdvancedClick,	//function
	75,

	419, 260, 89, 33,    // position
	FALSE,				//focus
	PUSH_BUTTON,	//type
	TRUE,				// visible
	BtnMouseDown,   //OnMouseDown()
	BtnMouseUp,		//OnMouseUp()
	BtnClick,		// OnClick()
	BtnKeyPress,	// OnKeyPress()
	PutButton,		// DisplayObject()
	TRUE,				//default button
	"View|Rates",	//text
	push,			//type
	up,				//state
	icon[14],           //icon
	ViewRateClick, //function
	76,

	418, 355, 89, 17,    // position
	FALSE,				//focus
	PUSH_BUTTON,	//type
	TRUE,				// visible
	BtnMouseDown,   //OnMouseDown()
	BtnMouseUp,		//OnMouseUp()
	BtnClick,		// OnClick()
	BtnKeyPress,	// OnKeyPress()
	PutButton,		// DisplayObject()
	FALSE,				//default button
	"\x9E Month \x9E",	//text
	push,			//type
	up,				//state
	NULL,           //icon
	billBackBtnClk,	//function
	77,

	438, 355, 89, 17,    // position
	FALSE,				//focus
	PUSH_BUTTON,	//type
	TRUE,				// visible
	BtnMouseDown,   //OnMouseDown()
	BtnMouseUp,		//OnMouseUp()
	BtnClick,		// OnClick()
	BtnKeyPress,	// OnKeyPress()
	PutButton,		// DisplayObject()
	FALSE,				//default button
	"\x9E Year \x9E",	//text
	push,			//type
	up,				//state
	NULL,           //icon
	billBackYrBtnClk,	//function
	77,

	418, 450, 89, 17,    // position
	FALSE,				//focus
	PUSH_BUTTON,	//type
	TRUE,				// visible
	BtnMouseDown,   //OnMouseDown()
	BtnMouseUp,		//OnMouseUp()
	BtnClick,		// OnClick()
	BtnKeyPress,	// OnKeyPress()
	PutButton,		// DisplayObject()
	FALSE,				//default button
	"\x9F Month \x9F",	//text
	push,			//type
	up,				//state
	NULL,           //icon
	billNextBtnClk,	//function
	78,

	438, 450, 89, 17,    // position
	FALSE,				//focus
	PUSH_BUTTON,	//type
	TRUE,				// visible
	BtnMouseDown,   //OnMouseDown()
	BtnMouseUp,		//OnMouseUp()
	BtnClick,		// OnClick()
	BtnKeyPress,	// OnKeyPress()
	PutButton,		// DisplayObject()
	FALSE,				//default button
	"\x9F Year \x9F",	//text
	push,			//type
	up,				//state
	NULL,           //icon
	billNextYrBtnClk,	//function
	78,

	419, 545, 89, 33,    // position
	FALSE,			//focus
	PUSH_BUTTON,	//type
	TRUE,			// visible
	BtnMouseDown,   //OnMouseDown()
	BtnMouseUp,		//OnMouseUp()
	BtnClick,		// OnClick()
	BtnKeyPress,	// OnKeyPress()
	PutButton,		// DisplayObject()
	TRUE,			//default button
	"Main|Menu",		//text
	push,			//type
	up,				//state
	icon[12],           //icon
	IntroScreen,	//function
	79
};      

SCREEN_OBJECT_T *billSO[9];

LWINDOW_T billWindow =	{	35, 0, 640, 445,	//window position
							billSO,		//screen object
							BillingStatement, // display window
							9,			//item count
							0,			//cursor position 0-79 left to right
							0,			//element that has current focus
							fcsBtn,		//focus type
							"Estimated Bill",
							DefaultOnKeyPress	//OnKeyPress();
						};							

static bill_month_index;

static void FixDates(DATE_ENTRY_T *de)
{
	int	leap;
	int	this_month;
	int	this_year;
	int	today;
	ULONG	now;
	ULONG	ed;
	ULONG	bd;
	struct tm	*tod;
	
	time(&ltime);
	tod = localtime(&ltime);
	this_month = tod->tm_mon+1;
	today = tod->tm_mday;
	this_year = tod->tm_year;

#if 1
	now =	today +
			this_month*100 +
			this_year * 10000L;
	bd =	de->begin_day +
			de->begin_month*100L +
			de->begin_year * 10000L;
#else
	if( (this_year % 4) == 0 )
	{
		leap = 1;
	}
	else
	{
		leap = 0;
	}
	now =	today +
			days_tab[leap][this_month-1] +
			(this_year * (365+leap));

	if( (de->begin_year % 4) == 0 )
	{
		leap = 1;
	}
	else
	{
		leap = 0;
	}
	if(day_tab[leap][de->begin_month] < de->begin_day)
	{
		de->begin_day = day_tab[leap][de->begin_month];
	}
	bd =	de->begin_day +
			days_tab[leap][de->begin_month-1] +
			(de->begin_year * (365+leap));
#endif

	if (bd > now)
	{
		de->begin_month--;
		if ( de->begin_month == 0)
		{
			de->begin_month = 12;
			--de->begin_year;
		}
		if ( (de->begin_year % 4) == 0)
		{
			leap = 1;
		}
		else
		{
			leap = 0;
		}
		if (de->begin_day > day_tab[leap][de->begin_month])
		{
			de->begin_day = day_tab[leap][de->begin_month];
		}
	}

	de->end_day = de->begin_day;
	de->end_month = de->begin_month+1;
	de->end_year = de->begin_year;

#if 1
	ed =	de->end_day +
			de->end_month * 100L +
			de->end_year * 10000L;
#else
	if( (de->end_year % 4) == 0 )
	{
		leap = 1;
	}
	else
	{
		leap = 0;
	}
	ed =	de->end_day +
			days_tab[leap][de->end_month-1] +
			(de->end_year * (365+leap));
#endif

	if (ed > now)
	{
		de->end_day = today;
		de->end_month = this_month;
		de->end_year = this_year;
	}
	else
	{
		if (de->end_month > 12)
		{
			bill_date.end_month = 1;
			++bill_date.end_year;
		}
		
		if ( (--de->end_day) == 0)
		{
			if ( (--de->end_month) == 0)
			{
				de->end_month = 12;
				--de->end_year;
			}
			if ( (de->end_year % 4) == 0)
			{
				leap = 1;
			}
			else
			{
				leap = 0;
			}
			de->end_day = day_tab[leap][de->end_month];
		}
	}
}

void BillingMenu(void)
{
    int		i;
    FILE *f;

	bill_month_index = 0;

	for (i = 0; i < billWindow.item_count-1; i++)
	{
		billSO[i] = &billBtnList[i].so;
	}
	billSO[billWindow.item_count-1]= &spin.so;

	if ((f=fopen("bill.dat", "rb")) != NULL)
	{
		fread( &spin.value, sizeof(int), 1, f );
		fclose(f);
	}
	else
	{
		spin.value = 1;
	}

	FirstHistory();	
	time(&ltime);
	tod = localtime(&ltime);
	bill_date.begin_day = spin.value;
	bill_date.begin_month = tod->tm_mon + 1;			// this month
	bill_date.begin_year = tod->tm_year;

	FixDates(&bill_date);
	glbWindow = &billWindow;
	PutWindow(&billWindow);
}   

//=====================================================================
// Please note: This month's est. bill is in bill_month_index == 0
//				LAST month's est. bill is in bill_month_index == 1
//				Therefore:  To get to the next month, SUBTRACT one
//				from the index, to get to the previous month, ADD
//				one to the index.  Trust me.
//=====================================================================
void billNextBtnClk(void)
{
	if ( (--bill_month_index) < 0)
	{
		bill_month_index = BILLING_STATEMENT_MAX;
	}
	bill_date.begin_day = spin.value;

	time(&ltime);
	tod = localtime(&ltime);
	if (bill_month_index == 0)
	{
		bill_date.begin_month = tod->tm_mon + 1;
		bill_date.begin_year = tod->tm_year;
		FixDates(&bill_date);
	}
	else
	{
		bill_date.begin_month = tod->tm_mon + 1 - bill_month_index;
		bill_date.begin_year = tod->tm_year;
		while(bill_date.begin_month  < 1)
		{
			bill_date.begin_year--;
			bill_date.begin_month += 12;
		}                 
		FixDates(&bill_date);
	}
	BillingStatement();
}

void billNextYrBtnClk(void)
{
	bill_month_index -= 12;
	if ( bill_month_index < 0)
	{
		bill_month_index = 0;
	}
	bill_date.begin_day = spin.value;

	time(&ltime);
	tod = localtime(&ltime);
	if (bill_month_index == 0)
	{
		bill_date.begin_month = tod->tm_mon + 1;
		bill_date.begin_year = tod->tm_year;
		FixDates(&bill_date);
	}
	else
	{
		bill_date.begin_month = tod->tm_mon + 1 - bill_month_index;
		bill_date.begin_year = tod->tm_year;
		while(bill_date.begin_month  < 1)
		{
			bill_date.begin_year--;
			bill_date.begin_month += 12;
		}                 
		FixDates(&bill_date);
	}
	BillingStatement();
}


//=====================================================================
// Please note: This month's est. bill is in bill_month_index == 0
//				LAST month's est. bill is in bill_month_index == 1
//				Therefore:  To get to the next month, SUBTRACT one
//				from the index, to get to the previous month, ADD
//				one to the index.  Trust me.
//=====================================================================
void billBackBtnClk(void)
{
	if ( (++bill_month_index) > BILLING_STATEMENT_MAX )
	{
		bill_month_index = 0;
	}
	time(&ltime);
	tod = localtime(&ltime);
	bill_date.begin_day = spin.value;
	bill_date.end_day = spin.value;

	if (bill_month_index == 0)
	{
		bill_date.begin_month = tod->tm_mon + 1;
		bill_date.begin_year = tod->tm_year;
		FixDates(&bill_date);
	}
	else
	{
		bill_date.begin_month = (tod->tm_mon + 1) - bill_month_index;
		bill_date.begin_year = tod->tm_year;
		while(bill_date.begin_month  < 1)
		{
			bill_date.begin_year--;
			bill_date.begin_month += 12;
		}                 
		FixDates(&bill_date);
	}
	BillingStatement();
}

void billBackYrBtnClk(void)
{
	bill_month_index += 12;

	if ( bill_month_index > BILLING_STATEMENT_MAX )
	{
		bill_month_index = BILLING_STATEMENT_MAX;
	}
	time(&ltime);
	tod = localtime(&ltime);
	bill_date.begin_day = spin.value;
	bill_date.end_day = spin.value;

	if (bill_month_index == 0)
	{
		bill_date.begin_month = tod->tm_mon + 1;
		bill_date.begin_year = tod->tm_year;
		FixDates(&bill_date);
	}
	else
	{
		bill_date.begin_month = (tod->tm_mon + 1) - bill_month_index;
		bill_date.begin_year = tod->tm_year;
		while(bill_date.begin_month  < 1)
		{
			bill_date.begin_year--;
			bill_date.begin_month += 12;
		}                 
		FixDates(&bill_date);
	}
	BillingStatement();
}

//==========================================================================
// BillingStatement()
//
// Routine History:
// Author					Date				Description
// MSL						04/09/97			Produces a billing statement
//												by interpreting a program
//												file written in our own language
// Inputs:
//  Name					Type				Description
//    -none-
//
// Returns:
//  Name					Type				Description
//  nothing
//
// Description:
//==========================================================================
void BillingStatement(void)
{
	UCHAR	line[256];
	UCHAR	in_line[256];
	UCHAR	buffer[256];
	int		line_count;
	FILE	*prgfile = NULL;

	memset(process_if, TRUE, 100 * sizeof(char));
	if_level = 0;	// not in an if statement

	current_line = 6;
	position(current_line, 1);
	line_count = 0;
	var_count = 0;
	array_count = 0;
	memset(variable, 0, sizeof(VARIABLE)*MAX_VARIABLES);
	memset(arrays, 0, sizeof(double)*256*MAX_ARRAYS);

	//=============================================
	// replace testload() with the calculations
	// from old billingstatement()
	//=============================================
	CalculateBill();
	
	hide_mouse();
	area_clear(60, 410, 5, 635, FG_WHT);
	
	if ( (prgfile = fopen("billing.prg", "rt")) == NULL)
	{
		unhide_mouse();
		msgbox("Cannot open file: billing.prg", "Warning", MB_OK);
		return;
	}

	while(!feof(prgfile))
	{
		// get a line from file and clean the comments from it
		// if nothin's left start over with next line
		Read(line, prgfile);
		strcpy(in_line, line);
		++line_count;
		if ( Clean(line) != 0 )
		{
			if (Process(line) == P_FAILURE)
			{
				trim(in_line);
				unhide_mouse();
				sprintf(buffer,"Syntax error at line # %d|%.25s", line_count, in_line);
				msgbox(buffer, "Error", MB_OK);
				break;
			}
		}
	}
	fclose(prgfile);
	ptext("First", 405, 5, BG_TRANSPARENT + FG_BLK);
	ptext("Day", 419, 5, BG_TRANSPARENT + FG_BLK);
	unhide_mouse();
}


//==========================================================================
// void Read(UCHAR *linein, FILE *prgfile)
//
// Routine History:
// Author					Date				Description
// MSL						04/09/97			Reads a line from incoming
//												program file
//
// Inputs:
//  Name					Type				Description
//   linein					UCHAR *				the incoming line buffer
//   prgfile				FILE *				the open file
//
// Returns:
//  Name					Type				Description
//  nothing
//
// Description:
//	Reads a line from the incoming program file
//==========================================================================
void Read(UCHAR *linein, FILE *prgfile)
{
	memset(linein, 0, 256);
	
	if (!feof(prgfile))
	{
		fgets(linein, 256, prgfile);
	}
}



//==========================================================================
// void Clean(UCHAR *line)
//
// Routine History:
// Author					Date				Description
// MSL						04/09/97			Strips the incoming program
//												line of comments and white
//												space.
//
// Inputs:
//  Name					Type				Description
//   line					UCHAR *				the incoming line
//
// Returns:
//  Name					Type				Description
//  lenght of new string	int					....
//
// Description:
//	Cleans the incoming program line of white space and comments,
//  converting non-quoted sections to upper case.
//==========================================================================
int Clean(UCHAR *line)
{
	UCHAR	*tok;
	ULONG	i;
	ULONG	j;
	UCHAR	newline[256];
	int		quote = 0;
	
	//strip comments
	if ( (tok = strstr(line, ";")) != NULL)
	{
		*tok = 0x00;
	}
	
	//strip white space leaving quoted text alone
	for (i = 0, j=0; i < strlen(line); i++)
	{
		if (quote == 0)
		{
			if ( line[i] > ' ' )
			{
				if (line[i] == '"' )
				{
					quote = 1;
				}
				newline[j++] = toupper(line[i]);
				newline[j] = 0;
			}
		}
		else			// searching for 2nd quotation mark
		{
			if (line[i] == '"')
			{
				quote = 0;
			}
			newline[j++] = line[i];
			newline[j] = 0;
		}
	}
	newline[j] = 0x00;
	strcpy(line, newline);
	return strlen(newline);
}


//==========================================================================
// Process(UCHAR *line)
//
// Routine History:
// Author					Date				Description
// MSL						04/09/97			Intreprets a line of the
//												user's program
//
// Inputs:
//  Name					Type				Description
//   line					UCHAR *				the incoming line, stripped of
//												white space, etc.
//
// Returns:
//  Name					Type				Description
//  Result code
//
// Description:
//	Intreprets a line of the user's program.
//==========================================================================
int Process(UCHAR *line)
{
	UCHAR	*bookmark;
	int		kw;
	int		i;
	int		tmp;
	char	*bm;
	double	value1;
	double	value2;

//char t[100];

	bookmark = line;
	

	kw = GetKeyWord(line, bookmark);
	if (kw == -1)
	{
		return P_FAILURE;
	}
	else
	{
//////////////////////////////////////////////////////////////
//sprintf(t, "BEFORE IL: %d\tP[-1]: %d\tP: %d\t\t%s", if_level,process_if[if_level-1], process_if[if_level], line);
//ExceptionLog(t);

		if (process_if[if_level] == TRUE)
		{
			switch(kw)
			{
				case 0:	//NEWLINE
					tmp = atoi(bookmark + 7);
					if (tmp == 0)
					{
						tmp = 1;
					}
					current_line += tmp;
					if (current_line > MAX_LINES)
					{
						return P_FAILURE;
					}
					position(current_line, 1);
					break;
				case 1:	//GOTO
					tmp = atoi(bookmark + 4);
					if (tmp > MAX_COLUMNS)
					{
						return P_FAILURE;
					}
					position( current_line, tmp);
					break;
				case 2:	//PRINT
					Print(bookmark);
					break;
				case 3:	//LINE
					i = atoi(bookmark+4);
					if ( (i > MAX_COLUMNS) || (i < 1))
					{
						return P_FAILURE;
					}
					bm = strstr(bookmark + 4, ",");
					tmp = atoi(bm+1);
					if ( (tmp > MAX_COLUMNS) || (tmp < i) )
					{
						return P_FAILURE;
					}
	
					hline((current_line * 14) - 7, (i-1)*8, 8 * tmp, FG_HWHT);
					hline((current_line * 14) - 8, (i-1)*8+1, 8 * tmp+1, FG_GRY);
	
					break;
				case 5:	//IF
					if_level++;
					if ( (bm = strstr(bookmark+2,"EQUALS")) != NULL)
					{
						*bm = 0;
						value1 = Evaluate(bookmark + 2);
						value2 = Evaluate(bm + 6);
						if (value1 == value2)
						{
							process_if[if_level] = TRUE;
						}
						else
						{
							process_if[if_level] = FALSE;
						}
					}
					else if ( (bm = strstr(bookmark+2,"ISNOTEQUAL")) != NULL)
					{
						*bm = 0;
						value1 = Evaluate(bookmark + 2);
						value2 = Evaluate(bm + 10);
						if (value1 != value2)
						{
							process_if[if_level] = TRUE;
						}
						else
						{
							process_if[if_level] = FALSE;
						}
					}
					else if ( (bm = strstr(bookmark+2,"ISLESSTHAN")) != NULL)
					{
						*bm = 0;
						value1 = Evaluate(bookmark + 2);
						value2 = Evaluate(bm + 10);
						if (value1 < value2)
						{
							process_if[if_level] = TRUE;
						}
						else
						{
							process_if[if_level] = FALSE;
						}
					}
					else if ( (bm = strstr(bookmark+2,"ISGREATERTHAN")) != NULL)
					{
						*bm = 0;
						value1 = Evaluate(bookmark + 2);
						value2 = Evaluate(bm + 13);
						if (value1 > value2)
						{
							process_if[if_level] = TRUE;
						}
						else
						{
							process_if[if_level] = FALSE;
						}
					}
					else
					{
						value1 = Evaluate(bookmark+2);
						if (value1 == 0)
						{
							process_if[if_level] = TRUE;
						}
						else
						{
							process_if[if_level] = FALSE;
						}
					}
					break;
				case 6:	//ELSE
					process_if[if_level] ^= TRUE;
					break;
				case 4:	//ENDIF
				    process_if[if_level] = TRUE;
				    if_level--;
					break;
				case 7:	//%
					if (ProcessVariable(bookmark) == P_FAILURE)
					{
						return P_FAILURE;
					}
					break;
				default:
					return P_FAILURE;
			}
		}
		else
		{
			switch(kw)
			{
				case 5:	//IF
					if_level++;
					process_if[if_level] = FALSE;
					break;
				case 6:	//ELSE
					// if parent level is true then toggle this level...
					if ( process_if[if_level-1] == TRUE )
					{
						process_if[if_level] ^= TRUE;
					}
					break;
				case 4:	//ENDIF
					process_if[if_level] = TRUE;
				    if_level--;
					break;
			}
		}
//sprintf(t, "AFTER  IL: %d\tP[-1]: %d\tP: %d\t\t%s", if_level,process_if[if_level-1], process_if[if_level], line);
//ExceptionLog(t);

		return P_SUCCESS;
	}	
}

//==============================================
// GetKeyWord() returns the index of the keyword 
// found in UCHAR* line
//==============================================
int GetKeyWord(UCHAR *line, UCHAR *bookmark)
{
	int i;

	for(i = 0; i < MAX_KEYWORDS; i++)
	{
		if( (bookmark = strstr(line, keywords[i])) != NULL)
		{
			return i;
		}
	}
	return -1;
}


//==========================================================================
// Print(UCHAR *what)
//
// Routine History:
// Author					Date				Description
// MSL						04/09/97			Performs a PRINT command
//
// Inputs:
//  Name					Type				Description
//   name					UCHAR *				argument for PRINT command
//
// Returns:
//  Name					Type				Description
//  Nothing
//
// Description:
//	Performs a PRINT command.  Takes the argument, intreprets it and
//  executes the command, if possible
//==========================================================================
void Print(UCHAR *str)
{
	char	work[80];
	ULONG	i;
	char	*bm;
	char	*var;
	char	*pt = NULL;
	char	output[256];
	char	buf[256];
	double	value;
	
	memset(output, 0, 256);
	memset(buf, 0, 256);
	
	if (str[5] == '"')
	{
		for (i = 0; i < strlen(&str[6]); i++)
		{
			if (str[i+6] == '"')
			{
				break;
			}
			sprintf(output, "%c", str[i+6]);
			text(output,BG_WHT+FG_BLK);
		}
	}
	else if (strcmp(str+5, "%MONTHNAME%") == 0)
	{
		strcpy(work, "%ENDMONTH%");
		text(month_name[(int)Evaluate(work)-1],BG_WHT+FG_BLK);
	}
	else 
	{
		bm = strstr(str,",");
		bm++;
		var = strstr(bm, ",");
		var++;
		value = Evaluate(var);
		sprintf(buf, "%%%d.%df", atoi(&str[5]), atoi(bm));	// create format string
		sprintf(output, buf, value);
		text(output,BG_WHT+FG_BLK);
	}
}

//==========================================================================
// ProcessVariable(UCHAR *variable_name)
//
// Routine History:
// Author					Date				Description
// MSL						04/09/97			Looks up a variable found
//												on the left side of an
//												assignment statement.
//
// Inputs:
//  Name					Type				Description
//   name					UCHAR *				variable's name
//
// Returns:
//  Name					Type				Description
//   result					int					Success or Failure
//
// Description:
//	Looks up a variable found on the left side of an assignment statement.
//  If the variable does not exist, it will be created.
//==========================================================================
int ProcessVariable(UCHAR *bookmark)
{
	char	tmpname[40];
	char	work[80];
	int		i;
	int		j;
	int		is_array;
	int		var_index;
	int 	var_found;
	int		index_value;

	var_found = FALSE;
	memset(tmpname, 0, 40);
	is_array = FALSE;

	//extract the var name
	for(i = 0, j = 0; i < (int)strlen(bookmark+1); i++)
	{
		if (bookmark[i+1] == '%')		// end of variable name
		{
			bookmark += (i + 2);
			if (memcmp(tmpname,"TBL_",4) == 0)		// is it a table??
			{
				is_array = 1;
				memset(work, 0, MAX_VARIABLE_NAME+1);
				for (i = 0; i < MAX_VARIABLE_NAME; ++i)
				{
					if ((*(bookmark + i)) == '<')		// expect <=
					{
						break;
					}
					if ((*(bookmark + i)) == 0)
					{
						break;
					}
					work[i] = *(bookmark + i);
				}
				index_value = (int)(Evaluate(work));
				bookmark += i;
			}
			break;
		}

		tmpname[j++] = bookmark[i+1];
		if (strlen(tmpname) > MAX_VARIABLE_NAME)
		{
			return (P_FAILURE);
		}
	}

	//look for variable in list
	for(i = 0; i < var_count; i++)
	{
		if (strcmp(tmpname, variable[i].name) == 0)
		{
			var_found = TRUE;
			var_index = i;
			break;
		}
	}

	if (var_found == FALSE)	//add var to list
	{
		if (var_count == MAX_VARIABLES)
		{
			sprintf(work, "Variable count exceeded|in billing.prg.|%d variables found.", var_count);
			msgbox(work,"Warning",MB_OK);
			return P_FAILURE;
		}
		for (i = 0; i < (int)strlen(tmpname); i++)
		{
			variable[var_count].name[i] = tmpname[i];
		}
		if (is_array != 0)
		{
			variable[var_count].type = 1;
			variable[var_count].index = array_count++;
			var_index = var_count;
			var_count++;
		}
		else
		{
			variable[var_count].type = 0;
			var_index = var_count;
			var_count++;
		}
	}
	
	if (memcmp(bookmark, "<=", 2) != 0)
	{
		return P_FAILURE;
	}
	if (is_array == 0)
	{
		variable[var_index].value = Evaluate(bookmark + 2);
	}
	else
	{
		arrays[index_value][variable[var_index].index] = Evaluate(bookmark + 2);
	}
	return (P_SUCCESS);
}


//==========================================================================
// GetValue(UCHAR *variable_name)
//
// Routine History:
// Author					Date				Description
// MSL						04/09/97			Looks up the value of a 
//												variable and returns the
//												variable's value
//
// Inputs:
//  Name					Type				Description
//   name					UCHAR *				variable's name
//
// Returns:
//  Name					Type				Description
//   value					double				current value of the variable
//
// Description:
//	Looks up the value of a variable and returns the 
//  variable's current value.
//==========================================================================

double GetValue(UCHAR *name)
{
	int i;

	for(i = 0; i < var_count; i++)
	{
		if (strcmp(name, variable[i].name) == 0)
		{
			return variable[i].value;
		}
	}
	return (0.0);
}


//==========================================================================
// GetTableValue(UCHAR *variable_name, UCHAR *index_str)
//
// Routine History:
// Author					Date				Description
// MSL						04/09/97			Looks up the value of a 
//												variable and returns the
//												variable's value
//
// Inputs:
//  Name					Type				Description
//   name					UCHAR *				variable's name
//
// Returns:
//  Name					Type				Description
//   value					double				current value of the variable
//
// Description:
//	Looks up the value of a variable and returns the 
//  variable's current value.
//==========================================================================
double GetTableValue(UCHAR *name, UCHAR *ndx_str)
{
	int i;
	int	ndx;

	ndx = (int)(Evaluate(ndx_str));

	for(i = 0; i < var_count; i++)
	{
		if (strcmp(name, variable[i].name) == 0)
		{
			return arrays[ndx][variable[i].index];
		}
	}
}


//==========================================================================
// Simplify(UCHAR *cp)
//
// Routine History:
// Author					Date				Description
// GES						04/09/97			Converts "+-" and "--" from
//												incoming string to "-" and
//												"+", respectively
//
// Inputs:
//  Name					Type				Description
//   cp						UCHAR *				string pointer
//
// Returns:
//  Name					Type				Description
//   cp						UCHAR *				simplified string
//
// Description:
//	Part of the parsing process_if.  Converts "+-" sequence to "-" AND
//  "--" to "+" in the incoming string
//==========================================================================
void Simplify(UCHAR *cp)
{
	char	*cp1;
	int		found;

	while (1)
	{
		found = FALSE;
		for (cp1 = cp; *cp1 != 0; ++cp1)				// strip "--" & "+-" out
		{
			if ( (*cp1 == '+') && (*(cp1+1) == '-'))
			{
				found = TRUE;
				strcpy(cp1, cp1+1);
				break;
			}
			if ( (*cp1 == '-') && (*(cp1+1) == '-'))
			{
				found = TRUE;
				strcpy(cp1, cp1+1);
				*cp1 = '+';
				break;
			}
		}
		if (found == FALSE)
		{
			break;
		}
	}
}


//==========================================================================
// AddSubtract(UCHAR *cp)
//
// Routine History:
// Author					Date				Description
// GES						04/09/97			Performs addition/subtraction
//												operations from a string and
//												puts the result back into the
//												string
//
// Inputs:
//  Name					Type				Description
//   cp						UCHAR *				string pointer
//
// Returns:
//  Name					Type				Description
//   cp						UCHAR *				simplified string
//
// Description:
//	Performs addition/subtraction operations specified in a string and
//  replaces the operation specification with the result.
//==========================================================================
void AddSubtract(UCHAR *cp)
{
	char	*cp1, *cp2, *cp3;
	char	work[80], work1[80];
	int		found;
	double	accumulator;

	while (1)
	{
		if ( (cp1 = strchr(cp+1, '-')) != NULL)
		{
			*cp1 = 0;
			found = FALSE;
			for( cp2 = cp1+1, cp3 = cp1-1; *cp2 != 0; ++cp2)	// find next operator
			{
				if ( (*cp2 == '+') || (*cp2 == '-') )
				{
					found = TRUE;
					while (cp3 >= cp)				// search for preceeding operator
					{
						if ( *cp3 == '+')
						{
							++cp3;
							break;
						}
						--cp3;
					}
					if (cp3 < cp)
					{
						cp3 = cp;
					}
					accumulator = atof(cp3);
					*cp3 = 0;
					accumulator -= atof(cp1+1);
					strcpy(work1, cp2);
					sprintf(work,"%f",accumulator);
					strcat(cp, work);
					strcat(cp, work1);
					cp2 = cp;
					break;
				}
			}
			if (found == FALSE)
			{
				cp3 = cp1 -1;
				while (cp3 >= cp)
				{
					if (*cp3 == '+')
					{
						++cp3;
						break;
					}
					--cp3;
				}
				if (cp3 < cp)
				{
					cp3 = cp;
				}
				accumulator = atof(cp3);
				*cp3 = 0;
				accumulator -= atof(cp1 + 1);
				sprintf(work,"%f",accumulator);
				strcat(cp, work);
				cp2 = cp;
			}
			Simplify(cp);
			continue;
		}
		else if ( (cp1 = strchr(cp, '+')) != NULL)
		{
			*cp1 = 0;
			for( cp2 = cp1+1; *cp2 != 0; ++cp2)	// search for next "+"
			{
				if ( *cp2 == '+')
				{
					accumulator = atof(cp);
					accumulator += atof(cp1+1);
					strcpy(work1, cp2);
					sprintf(work,"%f",accumulator);
					strcpy(cp, work);
					strcat(cp, work1);
				}
			}
			if ( (*cp2) == 0)
			{
				accumulator = atof(cp);
				accumulator += atof(cp1 + 1);
				sprintf(work,"%f",accumulator);
				strcpy(cp, work);
			}
			Simplify(cp);
			continue;
		}
		else
		{
			break;
		}
	}
}


//==========================================================================
// MultiplyDivide(UCHAR *cp)
//
// Routine History:
// Author					Date				Description
// GES						04/09/97			Performs multiplication/division
//												operations from a string and
//												puts the result back into the
//												string
//
// Inputs:
//  Name					Type				Description
//   cp						UCHAR *				string pointer
//
// Returns:
//  Name					Type				Description
//   cp						UCHAR *				simplified string
//
// Description:
//	Performs multiplication/division operations specified in the incoming
//  string and replaces the operation specification with the result.
//==========================================================================
void MultiplyDivide(UCHAR *cp)
{
	char	*cp1, *cp2, *cp3;
	char	work[80], work1[80];
	int		found;
	double	accumulator;

	while (1)
	{
		if ( (cp1 = strchr(cp+1, '*')) != NULL)
		{
			*cp1 = 0;
			found = FALSE;
			for( cp2 = cp1+1, cp3 = cp1 - 1; *cp2 != 0; ++cp2)	// search for "-" OR "+"
			{
				if ( (*cp2 == '+') || (*cp2 == '-') || (*cp2 == '*') || (*cp2 == '/') )
				{
					found = TRUE;
					while (cp3 >= cp)
					{
						if ( (*cp3 == '+') || (*cp3 == '-') || (*cp3 == '/') )
						{
							++cp3;
							break;
						}
						--cp3;
					}
					if (cp3 < cp)
					{
						cp3 = cp;
					}
					accumulator = atof(cp3);
					*cp3 = 0;
					accumulator *= atof(cp1+1);
					strcpy(work1, cp2);
					sprintf(work,"%f",accumulator);
					strcat(cp, work);
					strcat(cp, work1);
					cp2 = cp;
					break;
				}
			}
			if (found == FALSE)
			{
				cp3 = cp1 - 1;
				while (cp3 >= cp)
				{
					if ( (*cp3 == '+') || (*cp3 == '-') || (*cp3 == '/') )
					{
						++cp3;
						break;
					}
					--cp3;
				}
				if (cp3 < cp)
				{
					cp3 = cp;
				}
				accumulator = atof(cp3);
				*cp3 = 0;
				accumulator *= atof(cp1+1);
				sprintf(work,"%f",accumulator);
				strcat(cp, work);
				cp2 = cp;
			}
			Simplify(cp);
		}
		else if ( (cp1 = strchr(cp+1, '/')) != NULL)
		{
			*cp1 = 0;
			found = FALSE;
			for( cp2 = cp1+1, cp3 = cp1 - 1; *cp2 != 0; ++cp2)	// search for "-" OR "+"
			{
				if ( (*cp2 == '+') || (*cp2 == '-') || (*cp2 == '*') || (*cp2 == '/') )
				{
					found = TRUE;
					while (cp3 >= cp)
					{
						if ( (*cp3 == '+') || (*cp3 == '-') || (*cp3 == '/') )
						{
							++cp3;
							break;
						}
						--cp3;
					}
					if (cp3 < cp)
					{
						cp3 = cp;
					}
					accumulator = atof(cp3);
					*cp3 = 0;
					accumulator /= atof(cp1+1);
					strcpy(work1, cp2);
					sprintf(work,"%f",accumulator);
					strcat(cp, work);
					strcat(cp, work1);
					cp2 = cp;
					break;
				}
			}
			if (found == FALSE)
			{
				cp3 = cp1 - 1;
				while (cp3 >= cp)
				{
					if ( (*cp3 == '+') || (*cp3 == '-') || (*cp3 == '/') )
					{
						++cp3;
						break;
					}
					--cp3;
				}
				if (cp3 < cp)
				{
					cp3 = cp;
				}
				accumulator = atof(cp3);
				*cp3 = 0;
				accumulator /= atof(cp1+1);
				sprintf(work,"%f",accumulator);
				strcat(cp, work);
				cp2 = cp;
			}
			Simplify(cp);
		}
		else
		{
			break;
		}
	}
}


//==========================================================================
// Evaluate(UCHAR *cp)
//
// Routine History:
// Author					Date				Description
// GES						04/09/97			Parses incoming string and
//												evaluates it to return a
//												double precision value.
//
// Inputs:
//  Name					Type				Description
//   cp						UCHAR *				string pointer
//
// Returns:
//  Name					Type				Description
//   atof(cp)				double				result of parsing
//
// Description:
//	simplifies incoming string, replacing variables, multiplying, dividing,
//  adding and subtracting (being aware of parentheses) to return a double
//  precision result.
//==========================================================================
double Evaluate(UCHAR *cp)
{
	char	*cp1, *cp2, *cp3;
	char	work[80], work1[80];
	int		level;
	int		found;
	int		i;
	double	accumulator;

	//
	// First lookup variables & put their values into string to be parsed
	//
	while (1)
	{
		if ( (cp1 = strchr(cp, '%')) != NULL)
		{
			*cp1 = 0;			// null terminate beginning of string
			++cp1;				// point cp1 to beginning of variable name
			if ( (cp2 = strchr(cp1, '%')) != NULL )
			{											// variable name found 
				if (memcmp(cp1, "TBL_",4) == 0)			// is it a table
				{
					*cp2 = 0;
					++cp2;
					if (*cp2 == '%')					// index is a variable
					{
						cp3 = strchr(cp2+1,'%');
						if (cp3 == NULL)
						{
							return (0.0);
						}
						*cp3 = 0;
						++cp3;
						strcpy(work, cp2);
						strcat(work, "%");
						accumulator = GetTableValue(cp1, work);
						strcpy(work1, cp3);
						sprintf(work, "%f", accumulator);
						strcat(cp,work);
						strcat(cp,work1);
					}
					else								// index is a constant
					{
						for (cp3 = cp2, i = 0; *cp3 != 0; ++cp3)
						{
							memset(work, 0, 80);
							if ( isdigit(*cp3))
							{
								work[i++] = *cp3;
							}
							else
							{
								break;
							}
						}
						accumulator = GetTableValue(cp1, work);
						strcpy(work1, cp3);
						sprintf(work, "%f", accumulator);
						strcat(cp,work);
						strcat(cp,work1);
					}
				}
				else									// no, a plain ol variable
				{
					*cp2 = 0;
					accumulator = GetValue(cp1);
					++cp2;
					strcpy(work1, cp2);
					sprintf(work, "%f", accumulator);
					strcat(cp,work);
					strcat(cp,work1);
				}
			}
			else
			{
				// error condition
				return(0.0);
			}
		}
		else
		{
			break;
		}
	}

	// Change "--" to "+" AND "+-" to "-"
	Simplify(cp);

	// Second, remove all parentheses
	while (1)
	{
		if ( (cp1 = strchr(cp, '(')) != NULL)
		{
			found = FALSE;
			for( cp2 = cp1+1, level = 0; *cp2 != 0; ++cp2)	// search for matching ")"
			{
				if (*cp2 == '(')
				{
					++level;
					continue;
				}
				else if (*cp2 == ')')
				{
					if (level > 0)
					{
						--level;
						continue;
					}
					else
					{
						found = TRUE;
						*cp2 = 0;							// terminate inside paren.
						*cp1 = 0;							// terminate before paren.
						++cp1;
						++cp2;
						strcpy(work, cp1);					// inside paren.
						strcpy(work1, cp2);					// after paren.
						accumulator = Evaluate(work);		// evaluate what's inside
						sprintf(work, "%f", accumulator);   // value
						strcat(cp, work);
						strcat(cp, work1);
						break;
					}
				}
			}
			if (found == FALSE)
			{
				// error condition
				return (0.0);
			}
		}
		else
		{
			break;
		}
	}

	// Change "--" to "+" AND "+-" to "-"
	Simplify(cp);

	// Third perform Multiplication & Division operations
	MultiplyDivide(cp);

	// Change "--" to "+" AND "+-" to "-"
	Simplify(cp);

	// Finally do Addition/Subtraction
	AddSubtract(cp);

	return (atof(cp));
}

//==============================================================
//
//	OLD void CalculateBill(int month, int year, int begin_day) OLD
//	OLD void CalculateBill( DATE_ENTRY_T *date_entry ) OLD
//      void CalculateBill(void)
//
//	authors:	Mark Lauter & Gary Speegle
//	date:		14 May 97
//
//
//==============================================================
void CalculateBill(void)
{
	UCHAR	line[100];
	UCHAR	do_day;
	UCHAR	do_year;
	UCHAR	do_mon;
	UCHAR	rate_signal;

	char	end_key[40];
	char	found_key[40];
	char	id_string[40];
	
	int		i;
	int		lc_done;

	double	peak;
	double	rs_total_cost;
	double	rst_total_cost;
	double	rsv_total_cost;
	double	work;
	double	month_kwh;
	double	month_tod$[16];
	double	month_var$[16];
	double	lc_kwh_saved;
	double	lc_credit$;
	double	lc_kwh_used;
	double	lc_penalty$;
	double	last_kwh;
	double	tod_kwh_at[16];
	double	var_kwh_at[16];
	    
	DAYS_RATES	days_rates;
	
	DATE_ENTRY_T *date_entry;

	Busy();

	date_entry = &bill_date;

	memset((void *)&days_rates, 0, sizeof(DAYS_RATES));

	sprintf(	end_key,"FFFF%3d%2d%2d",
				date_entry->end_year,
				date_entry->end_month,
				date_entry->end_day);


	do_mon = date_entry->begin_month;
	do_day = date_entry->begin_day;
	do_year =date_entry->begin_year;

	// next, zero everything
	for( i = 0; i < 16; i++ )
	{
		tod_kwh_at[i] = 0.0;
		var_kwh_at[i] = 0.0;
		month_tod$[i] = 0.0;
		month_var$[i] = 0.0;
	}

	lc_kwh_saved = 0.0;
	lc_credit$ = 0.0;
	lc_penalty$ = 0.0;
	lc_kwh_used = 0.0;

	rs_total_cost = 0.0;
	rst_total_cost = 0.0;
	rsv_total_cost = 0.0;

	month_kwh = 0.0;

	peak = 0.0;

	sprintf(line, "%%EndMonth%%<=%d", date_entry->end_month);
	Process(strupr(line));

	sprintf(line, "%%StartDay%%<=%d", date_entry->begin_day);
	Process(strupr(line));
	sprintf(line, "%%StartMonth%%<=%d", date_entry->begin_month);
	Process(strupr(line));
	sprintf(line, "%%StartYear%%<=%d", date_entry->begin_year+1900);
	Process(strupr(line));
	sprintf(line, "%%EndDay%%<=%d", date_entry->end_day);
	Process(strupr(line));
	sprintf(line, "%%EndYear%%<=%d", date_entry->end_year+1900);
	Process(strupr(line));

	sprintf(line,"%%RateType%%<=%d", cust_info.rate_type);
	Process(strupr(line));
	sprintf(line,"%%FranchiseFeeType%%<=%d", cust_info.franchise_fee_index);
    Process(strupr(line));
	sprintf(line,"%%CityTaxType%%<=%d", cust_info.city_tax_index);
	Process(strupr(line));
	
	rs_total_cost = 0.0;
	rst_total_cost = 0.0;
	rsv_total_cost = 0.0;
	peak = 0.0;
	last_kwh = 0.0;

	if(open_history())
	{
		msgbox("History file not opened.","Warning", MB_OK);
		NotBusy();
		return;
	}
	
    pbutton(145, 200, 50, 240, PB_UP, FG_WHT);
    ptext("Estimating Bill...", 156, 240, BG_WHT+FG_BLU);

	
	//from 0 to last day in month
	while (1)
	{
		sprintf(found_key, "FFFF%3d%2d%2d    ",do_year, do_mon, do_day);
		strcpy(id_string, found_key);

		if ( memcmp(found_key, end_key, 11) > 0)
		{
			break;		// all done -- past end of cycle
		}

		if ( memcmp(found_key, end_key, 4) != 0)
		{
			break;		// all done -- not whole house anymore
		}

		switch (read_rates(do_mon, do_day, do_year, &days_rates))
		{     
			case 1:
			case 2:
			case 5:
				sprintf(line,
						"The rate information for|%02d/%02d/%4d is not available.",
						do_mon, do_day, do_year+1900);
				msgbox(line, "Information", MB_OK);
				close_history();
				NotBusy();
				return;
			case -1:
				close_history();
				NotBusy();
				return;
			case 0:
				break;
			default:
				break;
		}
		lc_done = FALSE;
	
		while (1)	// read one day from history.dat
		{
			if(next_history(found_key) != 0)
			{
				break;		// end of file
			}
			if(memcmp(found_key, id_string, 11) != 0)
			{
				break;		// another day or device;
			}

			work = hist_tran.ht_kwh;
			month_kwh += work;

			// calculate sliding peek
			if( (work + last_kwh) > peak)
			{      
				peak = work + last_kwh;
			}
			last_kwh = work;

			//accumulate kWh at this rate level for TOD and VAR
			//calculate rate signal (rate level 0-15) - TOD
			rate_signal = days_rates.rtod_signal[hist_tran.ht_hr/2];
			if (hist_tran.ht_hr & 1)	//odd hours in lower nibble
			{
				rate_signal &= 0x0F;
			}
			else	//even hours in upper nibble
			{
				rate_signal /= 16;
			}
			tod_kwh_at[rate_signal] += work;
			//calculate rate signal (rate level 0-15) - VAR
			rate_signal = days_rates.rsv_signal[hist_tran.ht_hr/2];
			if (hist_tran.ht_hr & 1)	//odd hours in lower nibble
			{
				rate_signal &= 0x0F;
			}
			else	//even hours in upper nibble
			{
				rate_signal /= 16;
			}
            var_kwh_at[rate_signal] += work;
			
			// calculate dollars for that hour
			rs_total_cost += work * ((double)days_rates.rs_rate)/100000.0;
			rst_total_cost += work * ((double)days_rates.rtod_rate[hist_tran.ht_hr])/100000.0;
			rsv_total_cost += work * ((double)days_rates.rsv_rate[hist_tran.ht_hr])/100000.0;
			month_tod$[rate_signal] += work * ((double)days_rates.rtod_rate[hist_tran.ht_hr])/100000.0;
			month_var$[rate_signal] += work * ((double)days_rates.rsv_rate[hist_tran.ht_hr])/100000.0;

			// calculate load control here...
			if (hist_tran.ht_flags & 0x01C)			// load control issued
			{
				lc_done = TRUE;
			}
		}
		if (lc_done == TRUE)
		{
			CalcLoadCtrlCreditPenalty(
					(int)do_mon,
					(int)do_day,
					(int)do_year,
					&days_rates,
					&lc_kwh_saved,
					&lc_credit$,
					&lc_kwh_used,
					&lc_penalty$);
		}
		next_day(&do_mon, &do_day, &do_year);
	}
	close_history();
	
	for( i = 0; i < 16; i++ )
	{
		sprintf(line,"%%MonthTodKWH@Level%d%%<=%f", i, tod_kwh_at[i]);
		Process(strupr(line));
		sprintf(line,"%%MonthVarKWH@Level%d%%<=%f", i, var_kwh_at[i]);
		Process(strupr(line));
		sprintf(line,"%%MonthTOD$@Level%d%%<=%f", i ,month_tod$[i]);
		Process(strupr(line));
		sprintf(line,"%%MonthVAR$@Level%d%%<=%f", i ,month_var$[i]);
		Process(strupr(line));
	}

	sprintf(line,"%%LoadControlKwhSaved%%<=%f", lc_kwh_saved);
	Process(strupr(line));
	sprintf(line,"%%LoadControlCredit$%%<=%f", lc_credit$);
	Process(strupr(line));
	sprintf(line,"%%LoadControlPenalty$%%<=%f", lc_penalty$);
	Process(strupr(line));
	sprintf(line,"%%LoadControlKwhUsed%%<=%f", lc_kwh_used);
	Process(strupr(line));

	sprintf(line,"%%MonthSTD$%%<=%f", rs_total_cost);
	Process(strupr(line));
	sprintf(line,"%%MonthTOD$%%<=%f", rst_total_cost);
	Process(strupr(line));
	sprintf(line,"%%MonthVAR$%%<=%f", rsv_total_cost);
	Process(strupr(line));

	sprintf(line,"%%MonthKWH%%<=%f", month_kwh);
	Process(strupr(line));               

	sprintf(line,"%%PeakDemand%%<=%f", peak);
	Process(strupr(line));
	
	area_clear(140, 210, 195, 445, FG_WHT);
	NotBusy();
}

void SaveDates(void)
{
	FILE *datefile;

	datefile = fopen("billdate.dat", "wb");
	fwrite(&date_count, sizeof(int), 1, datefile);
	fwrite(date_list, sizeof(DATE_ENTRY_T), MAX_DATES, datefile);
	fclose(datefile);
}

void OpenDates(void)
{
	FILE *datefile;
	memset(date_list, 0, MAX_DATES * sizeof(DATE_ENTRY_T));
	date_count = 0;
	if ((datefile = fopen("billdate.dat", "rb")) != NULL)
	{
		fread(&date_count, sizeof(int), 1, datefile);
		fread(date_list, sizeof(DATE_ENTRY_T), MAX_DATES, datefile);
		fclose(datefile);
	}
}

void CalcLoadCtrlCreditPenalty(
	int month,
	int day,
	int year,
	DAYS_RATES *days_rates,
	double	*lc_kwh_saved,
	double	*lc_credit$,
	double	*lc_kwh_used,
	double	*lc_penalty$)
{
	int i;
	char key[40];
	char key1[40];
	
	for (i = 0; i < dev_list.devices_used; ++i)
	{
		if ( IsSubmeter(&dev_list.devices[i]) && (dev_list.devices[i].priority > 0) )
		{
			sprintf(key, "%04X%3d%2d%2d    ",dev_list.devices[i].device_slot, year, month, day);
			memcpy(key1, key, 20);
			while (1)	// read one day from history.dat
			{
				if(next_history(key) != 0)
				{
					break;		// end of file
				}
				if(memcmp(key, key1, 11) != 0)
				{
					break;		// another day or device;
				}
	
				if (hist_tran.ht_flags & 0x040)			// credit given
				{
					*lc_kwh_saved += hist_tran.ht_kwh;
					*lc_credit$ +=  hist_tran.ht_kwh_cost * hist_tran.ht_kwh;
				}
				if (hist_tran.ht_flags & 0x020)			// penalty assessed
				{
					*lc_kwh_used += hist_tran.ht_kwh;
					*lc_penalty$ +=  hist_tran.ht_kwh_cost * hist_tran.ht_kwh;
				}
			}
		}
	}
}

static void AdvancedClick(void)
{
	//get the date list from disk
	OpenDates();
	AdvancedShow();
}

PUSH_BUTTON_T advancedbtn[5] = 
{
	367, 370, 70, 25,    // position
	FALSE,				//focus
	PUSH_BUTTON,	//type
	TRUE,				// visible
	BtnMouseDown,   //OnMouseDown()
	BtnMouseUp,		//OnMouseUp()
	BtnClick,		// OnClick()
	BtnKeyPress,	// OnKeyPress()
	PutButton,		// DisplayObject()
	FALSE,				//default button
	"Close",	//text
	push,			//type
	up,				//state
	NULL,           //icon
	BillingMenu,	//function
	80,

	340, 200, 70, 25,    // position
	FALSE,				//focus
	PUSH_BUTTON,	//type
	TRUE,				// visible
	BtnMouseDown,   //OnMouseDown()
	BtnMouseUp,		//OnMouseUp()
	BtnClick,		// OnClick()
	BtnKeyPress,	// OnKeyPress()
	PutButton,		// DisplayObject()
	FALSE,				//default button
	"View",	//text
	push,			//type
	up,				//state
	NULL,           //icon
	AdvancedBillView,	//function
	81,

	340, 272, 70, 25,    // position
	FALSE,				//focus
	PUSH_BUTTON,	//type
	TRUE,				// visible
	BtnMouseDown,   //OnMouseDown()
	BtnMouseUp,		//OnMouseUp()
	BtnClick,		// OnClick()
	BtnKeyPress,	// OnKeyPress()
	PutButton,		// DisplayObject()
	FALSE,				//default button
	"Add",	//text
	push,			//type
	up,				//state
	NULL,           //icon
	AddDate,	//function
	82,
	
	367, 200, 70, 25,    // position
	FALSE,				//focus
	PUSH_BUTTON,	//type
	TRUE,				// visible
	BtnMouseDown,   //OnMouseDown()
	BtnMouseUp,		//OnMouseUp()
	BtnClick,		// OnClick()
	BtnKeyPress,	// OnKeyPress()
	PutButton,		// DisplayObject()
	FALSE,				//default button
	"Change",	//text
	push,			//type
	up,				//state
	NULL,           //icon
	ChangeDate,	//function
	83,

	367, 272, 70, 25,    // position
	FALSE,				//focus
	PUSH_BUTTON,	//type
	TRUE,				// visible
	BtnMouseDown,   //OnMouseDown()
	BtnMouseUp,		//OnMouseUp()
	BtnClick,		// OnClick()
	BtnKeyPress,	// OnKeyPress()
	PutButton,		// DisplayObject()
	FALSE,				//default button
	"Remove",	//text
	push,			//type
	up,				//state
	NULL,           //icon
	RemoveDateEntry,	//function
	84
};
static void SortDateList(void);
SCREEN_OBJECT_T *advancedso[7];

LWINDOW_T advancedwindow =
{
	80, 195, 250, 320,	//window position
	advancedso,		//screen object
	NULL,
	7,			//item count
	0,			//cursor position 0-79 left to right
	0,			//element that has current focus
	fcsBtn,		//focus type
	"Advanced",
	DefaultOnKeyPress	//OnKeyPress();
};							

static void AdvancedShow(void)
{
	int i;

	advancedso[0] = &advancedbtn[0].so;
	advancedso[1] = &advancedbtn[1].so;
	advancedso[2] = &advancedbtn[2].so;
	advancedso[3] = &advancedbtn[3].so;
	advancedso[4] = &advancedbtn[4].so;
	advancedso[5] = &list_box.so;
	advancedso[6] = &advancedsb.so;
	advancedsb.max = list_box.item_count;
	SortDateList();
	list_box.item_count = date_count;
	for (i = 0; i < date_count; i++)
	{
		datesublist[i] = &date_list[i];
	}

	glbWindow = &advancedwindow;
	PutWindow(glbWindow);
}

static void DisplayDateEntry(DATE_ENTRY_T *de, int top, int left, UCHAR color)
{
	UCHAR text[81];

	memset(text, 0, 81);
	sprintf(text, 	"%02d/%02d/%04d - %02d/%02d/%04d", 
		de->begin_month,
		de->begin_day,
		de->begin_year+1900,
	    de->end_month,
	    de->end_day,
		de->end_year+1900
	);
	ptext(text, top, left, color);	
}

static void AddDateOkClick(void);
static void AddDateCancelClick(void);

static PUSH_BUTTON_T adddatebutton[2] =
{
	300, 247, 70, 30,
	TRUE,
	PUSH_BUTTON,
	TRUE,				// visible
	BtnMouseDown,
	BtnMouseUp,
	BtnClick,
	BtnKeyPress,
	PutButton,
	FALSE,
	"Ok",		//text
	push,
	up,
	NULL,
	AddDateOkClick,
	85,
											
	300, 247+75, 70, 30,
	FALSE,
	PUSH_BUTTON,
	TRUE,				// visible
	BtnMouseDown,
	BtnMouseUp,
	BtnClick,
	BtnKeyPress,
	PutButton,
	FALSE,
	"Cancel",		//text
	push,
	up,
	NULL,
	AddDateCancelClick,
	86
};
static void DateChange(void);
static TIME_ENTRY_T adddateentry1 =
{
	197,268,0,0,              // location
	FALSE,					// focus
	TIME_ENTRY,				// type
	TRUE,					// visible
	TimeEntryMouseDown,		// OnMouseDown()
	TimeEntryMouseUp,		// OnMouseUp()
	TimeEntryClick,			// OnClick()
	TimeEntryKeyPress,		// OnKeyPress()
	DisplayTimeEntry,		// DisplayObject()
	date_ent,				// type
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	DateChange
};

static TIME_ENTRY_T adddateentry2 =
{
	250,268,0,0,              // location
	FALSE,					// focus
	TIME_ENTRY,				// type
	TRUE,					// visible
	TimeEntryMouseDown,		// OnMouseDown()
	TimeEntryMouseUp,		// OnMouseUp()
	TimeEntryClick,			// OnClick()
	TimeEntryKeyPress,		// OnKeyPress()
	DisplayTimeEntry,		// DisplayObject()
	date_ent,				// type
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	DateChange
};

static void AddDateOkClick(void)
{
	long int d1;
	long int d2;

// validate operator's entry....
	d1 = adddateentry1.day
		 + (adddateentry1.month * 100L)
		 + (adddateentry1.year * 10000L);

	d2 = adddateentry2.day
		 + (adddateentry2.month * 100L)
		 + (adddateentry2.year * 10000L);

	if ( d1 >= d2)
	{
		msgbox("Ending date must be AFTER beginning date.","Error", MB_OK);
		return;
	}
	

// entry ok, process it
	if (acd == 0)	//add
	{
		if (date_count < MAX_DATES)
		{
			date_list[date_count].begin_day = adddateentry1.day;
			date_list[date_count].begin_month = adddateentry1.month;
			date_list[date_count].begin_year = adddateentry1.year;
		
			date_list[date_count].end_day = adddateentry2.day;
			date_list[date_count].end_month = adddateentry2.month;
			date_list[date_count].end_year = adddateentry2.year;
			date_count++;
		}
		else
		{
			msgbox("LaneView supports up to|24 advanced billing entries|two years of history.",
				"Advanced Billing", MB_OK);
		}
	}
	else	//change
	{
		date_list[list_box.item_selected].begin_day = adddateentry1.day;
		date_list[list_box.item_selected].begin_month = adddateentry1.month;
		date_list[list_box.item_selected].begin_year = adddateentry1.year;
	
		date_list[list_box.item_selected].end_day = adddateentry2.day;
		date_list[list_box.item_selected].end_month = adddateentry2.month;
		date_list[list_box.item_selected].end_year = adddateentry2.year;
	}
	SaveDates();
	AdvancedShow();
}

static void AddDateCancelClick(void)
{
	AdvancedShow();
}


static SCREEN_OBJECT_T *adddateso[4];

void static AddDateDraw(void);
LWINDOW_T adddatewindow = 
{
	140, 195, 250, 220,	//window position
	adddateso,			//screen object
	AddDateDraw,
	4,					//item count
	0,					//cursor position 0-79 left to right
	0,					//element that has current focus
	fcsBtn,				//focus type
	"Display Days Rates",	//title
	DefaultOnKeyPress	//OnKeyPress();
};							

static void ChangeDate(void)
{
	acd = 1;

	adddateso[0] = &adddatebutton[0].so;
	adddateso[1] = &adddatebutton[1].so;
	adddateso[2] = &adddateentry1.so;
	adddateso[3] = &adddateentry2.so;
	
	adddateentry1.month = date_list[list_box.item_selected].begin_month;
	adddateentry1.day = date_list[list_box.item_selected].begin_day;
	adddateentry1.year = date_list[list_box.item_selected].begin_year;

	adddateentry2.month = date_list[list_box.item_selected].end_month;
	adddateentry2.day = date_list[list_box.item_selected].end_day;
	adddateentry2.year = date_list[list_box.item_selected].end_year;

	glbWindow = &adddatewindow;
	PutWindow(&adddatewindow);
}

static void AddDateDraw(void)
{
    ptext("Enter period start date.",175,225,BG_TRANSPARENT + FG_BLK);
    ptext("Enter period end date.",230,225,BG_TRANSPARENT + FG_BLK);
}

static void AddDate(void)
{
	time_t		ltime;
	struct tm	*tod;

	acd = 0;

	adddateso[0] = &adddatebutton[0].so;
	adddateso[1] = &adddatebutton[1].so;
	adddateso[2] = &adddateentry1.so;
	adddateso[3] = &adddateentry2.so;
	
	time(&ltime);
	tod = localtime(&ltime);
	adddateentry1.month = tod->tm_mon+1;
	adddateentry1.day = tod->tm_mday;
	adddateentry1.year = tod->tm_year;

	adddateentry2.month = tod->tm_mon+1;
	adddateentry2.day = tod->tm_mday;
	adddateentry2.year = tod->tm_year;

	glbWindow = &adddatewindow;
	PutWindow(&adddatewindow);
    ptext("Enter period start date.",175,225,BG_TRANSPARENT + FG_BLK);
    ptext("Enter period end date.",230,225,BG_TRANSPARENT + FG_BLK);
}

static void AdvancedBillView(void)
{
	if (list_box.item_count < 1)
	{
		return;
	}
    
	bill_month_index = 0;
	
	bill_date.begin_day = date_list[list_box.item_selected].begin_day;
	bill_date.end_day = date_list[list_box.item_selected].end_day;
	bill_date.end_month = date_list[list_box.item_selected].end_month;
	bill_date.begin_month = date_list[list_box.item_selected].begin_month;
	bill_date.end_year = date_list[list_box.item_selected].end_year;
	bill_date.begin_year = date_list[list_box.item_selected].begin_year;

	glbWindow = &billWindow;
	PutWindow(&billWindow);
}

static void RemoveDateEntry(void)
{
	int i;
	
	if (date_count > 0)
	{
		for (i=list_box.item_selected; i<date_count-1; i++)
		{
			date_list[i] = date_list[i+1];
		}
		date_count--;
	
		SaveDates();
		
		list_box.item_count = date_count;
		for (i = 0; i < date_count; i++)
		{
			datesublist[i] = &date_list[i];
		}
		PutListBox(&list_box);
	}
}

static void DateChange(void)
{
	long int d1;
	long int d2;
	
	return;

	// make sure the end date is not before the begin date
#if 1
	d1 = adddateentry1.day
		 + (adddateentry1.month * 100L)
		 + (adddateentry1.year * 10000L);

	d2 = adddateentry2.day
		 + (adddateentry2.month * 100L)
		 + (adddateentry2.year * 10000L);
#else
	if ( (adddateentry1.year % 4) == 0)	// it's a leap year!!
	{
		d1 = adddateentry1.day
			 + (adddateentry1.month * day_tab[1][adddateentry1.month])
			 + (adddateentry1.year * 365);
	}
	else						// it's NOT a leap year!!
	{
		d1 = adddateentry1.day
			 + (adddateentry1.month * day_tab[0][adddateentry1.month])
			 + (adddateentry1.year * 365);
	}
	if ( (adddateentry2.year % 4) == 0)	// it's a leap year!!
	{
		d2 = adddateentry2.day
			 + (adddateentry2.month * day_tab[1][adddateentry2.month])
			 + (adddateentry2.year * 365);
	}
	else						// it's NOT a leap year!!
	{
		d2 = adddateentry2.day
			 + (adddateentry2.month * day_tab[0][adddateentry2.month])
			 + (adddateentry2.year * 365);
	}
#endif
	if ( d1 >= d2)
	{
		adddateentry2.day = adddateentry1.day + 1 ;
		adddateentry2.month = adddateentry1.month;
		adddateentry2.year = adddateentry1.year;
		DisplayTimeEntry(&adddateentry2);
	}
}

static void SortDateList(void)
{
	int i;
	ULONG	tmp_value;
	DATE_ENTRY_T tmp;
	ULONG datevalue[MAX_DATES];
	int swap;
	
	if (date_count > 1)
	{
		for ( i = 0; i < date_count; i++)
		{
			datevalue[i] =	date_list[i].begin_day +
							(date_list[i].begin_month * 100L) +
							(date_list[i].begin_year * 1000L);
		}
		swap = TRUE;
		while (swap == TRUE)
		{
			swap = FALSE;
			for ( i = 0; i < date_count - 1; i++)
			{
				if ( datevalue[i] > datevalue[i+1] )
				{
					memcpy(&tmp, &date_list[i], sizeof(DATE_ENTRY_T));
					memcpy(&date_list[i], &date_list[i+1], sizeof(DATE_ENTRY_T));
					memcpy(&date_list[i+1], &tmp, sizeof(DATE_ENTRY_T));
					tmp_value = datevalue[i];
					datevalue[i] = datevalue[i+1];
					datevalue[i+1] = tmp_value;
					swap = TRUE;
				}
			}
		}
	}
}

static void RefreshClick(void)
{
	bill_date.begin_day = spin.value;
	FixDates(&bill_date);
	BillingStatement();
}

static void ViewRateClick(void)
{
	Sender = BM_addr;
	ShowDaysRates();
}
