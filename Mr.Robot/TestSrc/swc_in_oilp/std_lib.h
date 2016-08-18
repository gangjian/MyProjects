/*	$RCSfile: std_lib.h $									*/
/*	$Date: 2015/10/26 14:17:36JST $							*/
/*	$Revision: 1.1 $										*/
/*	 EXPLANATION: ライブラリ(STDLIB) 公開ヘッダファイル		*/

#ifndef	STD_LIB_INC
#define	STD_LIB_INC

extern unsigned short WeightAvg(unsigned short, unsigned long*, unsigned short);		/* 加重平均値計算処理(16bit)		*/
extern unsigned char WeightAvgByte(unsigned char, unsigned short*, unsigned char);		/* 加重平均値計算処理(8bit)			*/
extern void BinToDec(unsigned long, unsigned char, unsigned char*);						/* 4バイトデータ10進化処理(BCD変換)	*/
extern unsigned long Divu_64_64_32(unsigned long, unsigned long, unsigned long, unsigned long);
																						/* 4バイトデータ除算処理			*/
extern void Mulu_64_32_64(unsigned long*, unsigned long*, unsigned long);				/* 4バイトデータ乗算処理			*/
extern void Add64_64_64(unsigned long*, unsigned long*, unsigned long, unsigned long);	/* 4バイトデータ加算処理			*/
extern unsigned short ByteOrderChange(unsigned short);									/* 2バイトオーダー変換処理			*/
extern void ShortToCharLFirst(unsigned char*, unsigned short*, unsigned short);			/* 2バイトオーダー変換処理(LSBファースト)*/
extern void ShortToCharUFirst(unsigned char*, unsigned short*, unsigned short);			/* 2バイトオーダー変換処理(MSBファースト)*/
extern void ConvergenceShort(unsigned short, unsigned short*, unsigned short);			/* 2バイトデータ収束処理			*/
extern void ConvergenceLong(unsigned long, unsigned long*, unsigned long);				/* 4バイトデータ収束処理			*/
extern unsigned short CalcCorrect(unsigned short, unsigned short, unsigned short);		/* データ補正計算処理				*/
extern void MemCpyChar(unsigned char*, unsigned char*, unsigned short);					/* 1バイトデータメモリコピー処理	*/
extern void MemCpyShort(unsigned short*, unsigned short*, unsigned short);				/* 2バイトデータメモリコピー処理	*/
extern void MemCpyLong(unsigned long*, unsigned long*, unsigned short);					/* 4バイトデータメモリコピー処理	*/
extern void MemSetChar(unsigned char*, unsigned char, unsigned short);					/* 1バイトデータメモリセット処理	*/
extern void MemSetShort(unsigned short*, unsigned short, unsigned short);				/* 2バイトデータメモリセット処理	*/
extern void MemSetLong(unsigned long*, unsigned long, unsigned short);					/* 4バイトデータメモリセット処理	*/
extern unsigned char CalcLsbCRC(unsigned char* , unsigned short);						/* LSB開始CRC計算処理				*/
extern unsigned char CalcMsbCRC(unsigned char* , unsigned short);						/* MSB開始CRC計算処理				*/
extern unsigned char MajorityChar(unsigned char*, unsigned char*, unsigned char*, unsigned char*);
																						/* 1バイトデータ単純多数決修復処理	*/
extern unsigned char MajorityLong(unsigned long*, unsigned long*, unsigned long*, unsigned long*);
																						/* 2バイトデータ単純多数決修復処理	*/
extern unsigned char MajorityShort(unsigned short*, unsigned short*, unsigned short*, unsigned short*);
																						/* 4バイトデータ単純多数決修復処理	*/
extern unsigned short ChkLvlParityShort(unsigned short*, unsigned short);				/* 水平パリティ確認処理				*/
extern unsigned short CalcLvlParityShort(unsigned short);								/* 水平パリティ計算処理				*/
extern unsigned short CalcVertiParityShort(unsigned short*, unsigned short);			/* 垂直パリティ計算処理				*/
extern unsigned short CalcDirectRatio(unsigned short, unsigned short, unsigned short, unsigned short, unsigned short);
																						/* 2点座標1次関数値計算処理			*/
extern unsigned short CalcSUM(unsigned short*, unsigned short);							/* サム計算処理						*/

#define CalcCheckSUM(data, len) (~(CalcSUM(data, len)))									/* チェックサム計算処理				*/

#endif	/* STD_LIB_INC */
