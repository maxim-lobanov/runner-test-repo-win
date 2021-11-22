package com.faskn.app.weatherapp.ui.dashboard.forecast

import androidx.databinding.ObservableField
import com.faskn.app.weatherapp.core.BaseViewModel
import com.faskn.app.weatherapp.domain.model.ListItem
import java.util.*
import javax.inject.Inject

/**
 * Created by Furkan on 2019-10-25
 */

class ForecastItemViewModel @Inject internal constructor() : BaseViewModel() {
    var item = ObservableField<ListItem>()
}
