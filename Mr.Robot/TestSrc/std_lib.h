/*	$RCSfile: std_lib.h $									*/
/*	$Date: 2015/10/26 14:17:36JST $							*/
/*	$Revision: 1.1 $										*/
/*	 EXPLANATION: ���C�u����(STDLIB) ���J�w�b�_�t�@�C��		*/

#ifndef	STD_LIB_INC
#define	STD_LIB_INC

extern unsigned short WeightAvg(unsigned short, unsigned long*, unsigned short);		/* ���d���ϒl�v�Z����(16bit)		*/
extern unsigned char WeightAvgByte(unsigned char, unsigned short*, unsigned char);		/* ���d���ϒl�v�Z����(8bit)			*/
extern void BinToDec(unsigned long, unsigned char, unsigned char*);						/* 4�o�C�g�f�[�^10�i������(BCD�ϊ�)	*/
extern unsigned long Divu_64_64_32(unsigned long, unsigned long, unsigned long, unsigned long);
																						/* 4�o�C�g�f�[�^���Z����			*/
extern void Mulu_64_32_64(unsigned long*, unsigned long*, unsigned long);				/* 4�o�C�g�f�[�^��Z����			*/
extern void Add64_64_64(unsigned long*, unsigned long*, unsigned long, unsigned long);	/* 4�o�C�g�f�[�^���Z����			*/
extern unsigned short ByteOrderChange(unsigned short);									/* 2�o�C�g�I�[�_�[�ϊ�����			*/
extern void ShortToCharLFirst(unsigned char*, unsigned short*, unsigned short);			/* 2�o�C�g�I�[�_�[�ϊ�����(LSB�t�@�[�X�g)*/
extern void ShortToCharUFirst(unsigned char*, unsigned short*, unsigned short);			/* 2�o�C�g�I�[�_�[�ϊ�����(MSB�t�@�[�X�g)*/
extern void ConvergenceShort(unsigned short, unsigned short*, unsigned short);			/* 2�o�C�g�f�[�^��������			*/
extern void ConvergenceLong(unsigned long, unsigned long*, unsigned long);				/* 4�o�C�g�f�[�^��������			*/
extern unsigned short CalcCorrect(unsigned short, unsigned short, unsigned short);		/* �f�[�^�␳�v�Z����				*/
extern void MemCpyChar(unsigned char*, unsigned char*, unsigned short);					/* 1�o�C�g�f�[�^�������R�s�[����	*/
extern void MemCpyShort(unsigned short*, unsigned short*, unsigned short);				/* 2�o�C�g�f�[�^�������R�s�[����	*/
extern void MemCpyLong(unsigned long*, unsigned long*, unsigned short);					/* 4�o�C�g�f�[�^�������R�s�[����	*/
extern void MemSetChar(unsigned char*, unsigned char, unsigned short);					/* 1�o�C�g�f�[�^�������Z�b�g����	*/
extern void MemSetShort(unsigned short*, unsigned short, unsigned short);				/* 2�o�C�g�f�[�^�������Z�b�g����	*/
extern void MemSetLong(unsigned long*, unsigned long, unsigned short);					/* 4�o�C�g�f�[�^�������Z�b�g����	*/
extern unsigned char CalcLsbCRC(unsigned char* , unsigned short);						/* LSB�J�nCRC�v�Z����				*/
extern unsigned char CalcMsbCRC(unsigned char* , unsigned short);						/* MSB�J�nCRC�v�Z����				*/
extern unsigned char MajorityChar(unsigned char*, unsigned char*, unsigned char*, unsigned char*);
																						/* 1�o�C�g�f�[�^�P���������C������	*/
extern unsigned char MajorityLong(unsigned long*, unsigned long*, unsigned long*, unsigned long*);
																						/* 2�o�C�g�f�[�^�P���������C������	*/
extern unsigned char MajorityShort(unsigned short*, unsigned short*, unsigned short*, unsigned short*);
																						/* 4�o�C�g�f�[�^�P���������C������	*/
extern unsigned short ChkLvlParityShort(unsigned short*, unsigned short);				/* �����p���e�B�m�F����				*/
extern unsigned short CalcLvlParityShort(unsigned short);								/* �����p���e�B�v�Z����				*/
extern unsigned short CalcVertiParityShort(unsigned short*, unsigned short);			/* �����p���e�B�v�Z����				*/
extern unsigned short CalcDirectRatio(unsigned short, unsigned short, unsigned short, unsigned short, unsigned short);
																						/* 2�_���W1���֐��l�v�Z����			*/
extern unsigned short CalcSUM(unsigned short*, unsigned short);							/* �T���v�Z����						*/

#define CalcCheckSUM(data, len) (~(CalcSUM(data, len)))									/* �`�F�b�N�T���v�Z����				*/

#endif	/* STD_LIB_INC */
