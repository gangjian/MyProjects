/*	$RCSfile: bsw.h $							*/
/*	$Date: 2015/11/12 13:08:00JST $				*/
/*	$Revision: 1.3 $							*/
/*	 EXPLANATION: BSW�p�w�b�_�t�@�C��			*/

#ifndef BSW_INC
#define BSW_INC

#define	BSW_DI()														/*	�����݋֎~		*/
#define	BSW_EI()														/*	�����݋���		*/
#define	BSW_NOP()														/*	����������		*/

#define	BSW_BITNUM_BYTE					(8)								/*	1�o�C�g�̃r�b�g��	*/
#define	BSW_BITNUM_WORD					(16)							/*	2�o�C�g�̃r�b�g��	*/
#define	BSW_BITNUM_DWORD				(32)							/*	4�o�C�g�̃r�b�g��	*/

#ifndef	NULL
#define	NULL							((void *)0)						/*	NULL�|�C���^	*/
#endif	/* NULL */

#endif	/*	BSW_INC	*/

