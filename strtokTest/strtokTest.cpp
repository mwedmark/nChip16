// strtokTest.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include "string.h"

int main()
{
    //std::cout << "Hello World!\n";
    
    char str[255] = "@0 //Simple test for LDI 1-R0=50,R1=51,C=0,$1234=1234";
    char tokenList[255][32];
    int numOfTokens = 0;
    printf("%s\r\n\r\n",str);
    char * token = strtok(str, "//,-=");
    while (token != NULL) 
    {
        printf(" %s\n", token);
        token = strtok(NULL, "//,-=");
        if (token != NULL)
        {
            strcpy(tokenList[numOfTokens++], token);
        }
    }

    // token list created, now lets interpret it!
    for (int i = 0; i < numOfTokens; i++)
    {
        if (tokenList[i][0] == 'R') // register assert
        {
            char string[2];
            string[0] = tokenList[i][1];
            string[1] = 0;
            int regNum = strtol(string, NULL, 16);
            i++;
            int regValue = strtol(tokenList[i], NULL, 10);
            printf("assert: R%0X=%0d\r\n", regNum, regValue);
        }
        if (tokenList[i][0] == 'C' || tokenList[i][0] == 'O' || tokenList[i][0] == 'Z' || tokenList[i][0] == 'V')
        {
            char flag = tokenList[i][0];
            i++;
            int flagValue = strtol(tokenList[i], NULL, 10);
            printf("assert: %0s=%0d\r\n", flag, flagValue);
            i++;
        }
    }
}

// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file
