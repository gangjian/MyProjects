/*	$RCSfile: bsw.h $							*/
/*	$Date: 2015/11/12 13:08:00JST $				*/
/*	$Revision: 1.3 $							*/
/*	 EXPLANATION: BSW用ヘッダファイル			*/

#ifndef BSW_INC
#define BSW_INC

#define	BSW_DI()														/*	割込み禁止		*/
#define	BSW_EI()														/*	割込み許可		*/
#define	BSW_NOP()														/*	無処理命令		*/

#define	BSW_BITNUM_BYTE					(8)								/*	1バイトのビット数	*/
#define	BSW_BITNUM_WORD					(16)							/*	2バイトのビット数	*/
#define	BSW_BITNUM_DWORD				(32)							/*	4バイトのビット数	*/

#ifndef	NULL
#define	NULL							((void *)0)						/*	NULLポインタ	*/
#endif	/* NULL */

#endif	/*	BSW_INC	*/

