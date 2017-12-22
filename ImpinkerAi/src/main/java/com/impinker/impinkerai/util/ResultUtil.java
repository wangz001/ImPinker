package com.impinker.impinkerai.util;

import com.impinker.impinkerai.enums.ResultEnum;
import com.impinker.impinkerai.vo.ResultVo;

/**
 * 返回数据通用类
 */
public class ResultUtil {


    public static ResultVo success(Object data){

        ResultVo resultVo=new ResultVo();
        resultVo.setData(data);
        resultVo.setCode(ResultEnum.SUCCESS.getCode());
        resultVo.setMsg(ResultEnum.SUCCESS.getMessage());
        return resultVo;
    }

    public static ResultVo success(){
        return success(null);
    }

    public static ResultVo error(ResultEnum resultEnum) {
        ResultVo resultVO = new ResultVo();
        resultVO.setCode(resultEnum.getCode());
        resultVO.setMsg(resultEnum.getMessage());
        return resultVO;
    }

}
