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

static int acd;	// add or change date flag for advanced functions

typedef struct
{
	UCHAR name[26];	// allows 25 char name
	UCHAR type;		//0 = single element, 1 = array memeber
	UCHAR index;	//index from 0
	double value;
}VARIABLE;

VARIABLE variable[MAX_VARIABLES];

double	arrays[256][MAX_ARRAYS];
int		array_count;
int var_count;

UCHAR* month_name[12] =
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

UCHAR* keywords[MAX_KEYWORDS] =
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
UCHAR* operators[MAX_OPERATORS] =
{
	"(",
	")",
	"*",
	"/",
	"+",
	"-",
	"%"
};

extern struct tm* tod; // time of day

#define MAX_DATES 100
DATE_ENTRY_T date_list[MAX_DATES];
int date_count;

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
	FILE* prgfile = NULL;

	memset(process_if, TRUE, 100 * sizeof(char));
	if_level = 0;	// not in an if statement

	current_line = 6;
	position(current_line, 1);
	line_count = 0;
	var_count = 0;
	array_count = 0;
	memset(variable, 0, sizeof(VARIABLE) * MAX_VARIABLES);
	memset(arrays, 0, sizeof(double) * 256 * MAX_ARRAYS);

	//=============================================
	// replace testload() with the calculations
	// from old billingstatement()
	//=============================================
	CalculateBill();

	hide_mouse();
	area_clear(60, 410, 5, 635, FG_WHT);

	if ((prgfile = fopen("billing.prg", "rt")) == NULL)
	{
		unhide_mouse();
		msgbox("Cannot open file: billing.prg", "Warning", MB_OK);
		return;
	}

	while (!feof(prgfile))
	{
		// get a line from file and clean the comments from it
		// if nothin's left start over with next line
		Read(line, prgfile);
		strcpy(in_line, line);
		++line_count;
		if (Clean(line) != 0)
		{
			if (Process(line) == P_FAILURE)
			{
				trim(in_line);
				unhide_mouse();
				sprintf(buffer, "Syntax error at line # %d|%.25s", line_count, in_line);
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
void Read(UCHAR* linein, FILE* prgfile)
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
int Clean(UCHAR* line)
{
	UCHAR* tok;
	ULONG	i;
	ULONG	j;
	UCHAR	newline[256];
	int		quote = 0;

	//strip comments
	if ((tok = strstr(line, ";")) != NULL)
	{
		*tok = 0x00;
	}

	//strip white space leaving quoted text alone
	for (i = 0, j = 0; i < strlen(line); i++)
	{
		if (quote == 0)
		{
			if (line[i] > ' ')
			{
				if (line[i] == '"')
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
int Process(UCHAR* line)
{
	UCHAR* bookmark;
	int		kw;
	int		i;
	int		tmp;
	char* bm;
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
			switch (kw)
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
				position(current_line, tmp);
				break;
			case 2:	//PRINT
				Print(bookmark);
				break;
			case 3:	//LINE
				i = atoi(bookmark + 4);
				if ((i > MAX_COLUMNS) || (i < 1))
				{
					return P_FAILURE;
				}
				bm = strstr(bookmark + 4, ",");
				tmp = atoi(bm + 1);
				if ((tmp > MAX_COLUMNS) || (tmp < i))
				{
					return P_FAILURE;
				}

				hline((current_line * 14) - 7, (i - 1) * 8, 8 * tmp, FG_HWHT);
				hline((current_line * 14) - 8, (i - 1) * 8 + 1, 8 * tmp + 1, FG_GRY);

				break;
			case 5:	//IF
				if_level++;
				if ((bm = strstr(bookmark + 2, "EQUALS")) != NULL)
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
				else if ((bm = strstr(bookmark + 2, "ISNOTEQUAL")) != NULL)
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
				else if ((bm = strstr(bookmark + 2, "ISLESSTHAN")) != NULL)
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
				else if ((bm = strstr(bookmark + 2, "ISGREATERTHAN")) != NULL)
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
					value1 = Evaluate(bookmark + 2);
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
			switch (kw)
			{
			case 5:	//IF
				if_level++;
				process_if[if_level] = FALSE;
				break;
			case 6:	//ELSE
				// if parent level is true then toggle this level...
				if (process_if[if_level - 1] == TRUE)
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
int GetKeyWord(UCHAR* line, UCHAR* bookmark)
{
	int i;

	for (i = 0; i < MAX_KEYWORDS; i++)
	{
		if ((bookmark = strstr(line, keywords[i])) != NULL)
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
void Print(UCHAR* str)
{
	char	work[80];
	ULONG	i;
	char* bm;
	char* var;
	char* pt = NULL;
	char	output[256];
	char	buf[256];
	double	value;

	memset(output, 0, 256);
	memset(buf, 0, 256);

	if (str[5] == '"')
	{
		for (i = 0; i < strlen(&str[6]); i++)
		{
			if (str[i + 6] == '"')
			{
				break;
			}
			sprintf(output, "%c", str[i + 6]);
			text(output, BG_WHT + FG_BLK);
		}
	}
	else if (strcmp(str + 5, "%MONTHNAME%") == 0)
	{
		strcpy(work, "%ENDMONTH%");
		text(month_name[(int)Evaluate(work) - 1], BG_WHT + FG_BLK);
	}
	else
	{
		bm = strstr(str, ",");
		bm++;
		var = strstr(bm, ",");
		var++;
		value = Evaluate(var);
		sprintf(buf, "%%%d.%df", atoi(&str[5]), atoi(bm));	// create format string
		sprintf(output, buf, value);
		text(output, BG_WHT + FG_BLK);
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
int ProcessVariable(UCHAR* bookmark)
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
	for (i = 0, j = 0; i < (int)strlen(bookmark + 1); i++)
	{
		if (bookmark[i + 1] == '%')		// end of variable name
		{
			bookmark += (i + 2);
			if (memcmp(tmpname, "TBL_", 4) == 0)		// is it a table??
			{
				is_array = 1;
				memset(work, 0, MAX_VARIABLE_NAME + 1);
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

		tmpname[j++] = bookmark[i + 1];
		if (strlen(tmpname) > MAX_VARIABLE_NAME)
		{
			return (P_FAILURE);
		}
	}

	//look for variable in list
	for (i = 0; i < var_count; i++)
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
			msgbox(work, "Warning", MB_OK);
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
double GetValue(UCHAR* name)
{
	int i;

	for (i = 0; i < var_count; i++)
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
double GetTableValue(UCHAR* name, UCHAR* ndx_str)
{
	int i;
	int	ndx;

	ndx = (int)(Evaluate(ndx_str));

	for (i = 0; i < var_count; i++)
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
void Simplify(UCHAR* cp)
{
	char* cp1;
	int		found;

	while (1)
	{
		found = FALSE;
		for (cp1 = cp; *cp1 != 0; ++cp1)				// strip "--" & "+-" out
		{
			if ((*cp1 == '+') && (*(cp1 + 1) == '-'))
			{
				found = TRUE;
				strcpy(cp1, cp1 + 1);
				break;
			}
			if ((*cp1 == '-') && (*(cp1 + 1) == '-'))
			{
				found = TRUE;
				strcpy(cp1, cp1 + 1);
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
void AddSubtract(UCHAR* cp)
{
	char* cp1, * cp2, * cp3;
	char	work[80], work1[80];
	int		found;
	double	accumulator;

	while (1)
	{
		if ((cp1 = strchr(cp + 1, '-')) != NULL)
		{
			*cp1 = 0;
			found = FALSE;
			for (cp2 = cp1 + 1, cp3 = cp1 - 1; *cp2 != 0; ++cp2)	// find next operator
			{
				if ((*cp2 == '+') || (*cp2 == '-'))
				{
					found = TRUE;
					while (cp3 >= cp)				// search for preceeding operator
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
					strcpy(work1, cp2);
					sprintf(work, "%f", accumulator);
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
				sprintf(work, "%f", accumulator);
				strcat(cp, work);
				cp2 = cp;
			}
			Simplify(cp);
			continue;
		}
		else if ((cp1 = strchr(cp, '+')) != NULL)
		{
			*cp1 = 0;
			for (cp2 = cp1 + 1; *cp2 != 0; ++cp2)	// search for next "+"
			{
				if (*cp2 == '+')
				{
					accumulator = atof(cp);
					accumulator += atof(cp1 + 1);
					strcpy(work1, cp2);
					sprintf(work, "%f", accumulator);
					strcpy(cp, work);
					strcat(cp, work1);
				}
			}
			if ((*cp2) == 0)
			{
				accumulator = atof(cp);
				accumulator += atof(cp1 + 1);
				sprintf(work, "%f", accumulator);
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
void MultiplyDivide(UCHAR* cp)
{
	char* cp1, * cp2, * cp3;
	char	work[80], work1[80];
	int		found;
	double	accumulator;

	while (1)
	{
		if ((cp1 = strchr(cp + 1, '*')) != NULL)
		{
			*cp1 = 0;
			found = FALSE;
			for (cp2 = cp1 + 1, cp3 = cp1 - 1; *cp2 != 0; ++cp2)	// search for "-" OR "+"
			{
				if ((*cp2 == '+') || (*cp2 == '-') || (*cp2 == '*') || (*cp2 == '/'))
				{
					found = TRUE;
					while (cp3 >= cp)
					{
						if ((*cp3 == '+') || (*cp3 == '-') || (*cp3 == '/'))
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
					accumulator *= atof(cp1 + 1);
					strcpy(work1, cp2);
					sprintf(work, "%f", accumulator);
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
					if ((*cp3 == '+') || (*cp3 == '-') || (*cp3 == '/'))
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
				accumulator *= atof(cp1 + 1);
				sprintf(work, "%f", accumulator);
				strcat(cp, work);
				cp2 = cp;
			}
			Simplify(cp);
		}
		else if ((cp1 = strchr(cp + 1, '/')) != NULL)
		{
			*cp1 = 0;
			found = FALSE;
			for (cp2 = cp1 + 1, cp3 = cp1 - 1; *cp2 != 0; ++cp2)	// search for "-" OR "+"
			{
				if ((*cp2 == '+') || (*cp2 == '-') || (*cp2 == '*') || (*cp2 == '/'))
				{
					found = TRUE;
					while (cp3 >= cp)
					{
						if ((*cp3 == '+') || (*cp3 == '-') || (*cp3 == '/'))
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
					accumulator /= atof(cp1 + 1);
					strcpy(work1, cp2);
					sprintf(work, "%f", accumulator);
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
					if ((*cp3 == '+') || (*cp3 == '-') || (*cp3 == '/'))
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
				accumulator /= atof(cp1 + 1);
				sprintf(work, "%f", accumulator);
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
double Evaluate(UCHAR* cp)
{
	char* cp1, * cp2, * cp3;
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
		if ((cp1 = strchr(cp, '%')) != NULL)
		{
			*cp1 = 0;			// null terminate beginning of string
			++cp1;				// point cp1 to beginning of variable name
			if ((cp2 = strchr(cp1, '%')) != NULL)
			{											// variable name found 
				if (memcmp(cp1, "TBL_", 4) == 0)			// is it a table
				{
					*cp2 = 0;
					++cp2;
					if (*cp2 == '%')					// index is a variable
					{
						cp3 = strchr(cp2 + 1, '%');
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
						strcat(cp, work);
						strcat(cp, work1);
					}
					else								// index is a constant
					{
						for (cp3 = cp2, i = 0; *cp3 != 0; ++cp3)
						{
							memset(work, 0, 80);
							if (isdigit(*cp3))
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
						strcat(cp, work);
						strcat(cp, work1);
					}
				}
				else									// no, a plain ol variable
				{
					*cp2 = 0;
					accumulator = GetValue(cp1);
					++cp2;
					strcpy(work1, cp2);
					sprintf(work, "%f", accumulator);
					strcat(cp, work);
					strcat(cp, work1);
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
		if ((cp1 = strchr(cp, '(')) != NULL)
		{
			found = FALSE;
			for (cp2 = cp1 + 1, level = 0; *cp2 != 0; ++cp2)	// search for matching ")"
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
