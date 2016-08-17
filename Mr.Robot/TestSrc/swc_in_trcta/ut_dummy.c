/*
	Dummy.c
*/
/*------------------------------------------------------------------------------*/
/*	インクルードファイル														*/
/*------------------------------------------------------------------------------*/
#include "ut_dummy.h"

/*------------------------------------------------------------------------------*/
/*	ダミー関数																	*/
/*------------------------------------------------------------------------------*/
static uint8 dmy_ShareLibStepFailJudgeVal_Cnt;
static VAR_IN_STEP dmy_ShareLibStepFailJudgeVal_varIn;
static MNG_STEP_VAL dmy_ShareLibStepFailJudgeVal_mngData;
static uint8 dmy_ShareLibStepFailJudgeVal_mngData_chgTbl[3];
static VAR_OUT_STEP dmy_ShareLibStepFailJudgeVal_varOut;
void ShareLibStepFailJudgeVal(const VAR_IN_STEP *varIn, const MNG_STEP_VAL *mngData, VAR_OUT_STEP *varOut)
{
	int i;
	
	dmy_ShareLibStepFailJudgeVal_Cnt++;
	dmy_ShareLibStepFailJudgeVal_varIn.inSig = varIn->inSig;
	dmy_ShareLibStepFailJudgeVal_varIn.msgSts = varIn->msgSts;
	dmy_ShareLibStepFailJudgeVal_varIn.powerSts = varIn->powerSts;

	dmy_ShareLibStepFailJudgeVal_mngData.chgTblSize = mngData->chgTblSize;
	dmy_ShareLibStepFailJudgeVal_mngData.pvInitVal = mngData->pvInitVal;
	dmy_ShareLibStepFailJudgeVal_mngData.failVal = mngData->failVal;
	dmy_ShareLibStepFailJudgeVal_mngData.outRangeVal = mngData->outRangeVal;

	for (i = 0; i < mngData->chgTblSize; i++)
	{
		dmy_ShareLibStepFailJudgeVal_mngData_chgTbl[i] = mngData->chgTbl[i];
	}
	varOut->pv = dmy_ShareLibStepFailJudgeVal_varOut.pv;
	varOut->pvSts = dmy_ShareLibStepFailJudgeVal_varOut.pvSts;
}

CONSTP2CONST(struct Rte_CDS_swc_in_trcta, RTE_CONST, RTE_APPL_CONST) Rte_Inst_swc_in_trcta;


/*
	* $History: $
	* $NoLgtwords: $
*/

